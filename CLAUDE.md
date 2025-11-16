# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

HarborMasterNice is a C# .NET 8.0 Windows Forms application for managing harbor operations, including ship arrivals, berth assignments, and scheduling. The application uses Supabase (PostgreSQL) as its backend database.

## Build and Run Commands

### Build the Project
```bash
dotnet build HarborMasterNice.sln
```

### Run the Application
```bash
dotnet run --project HarborMasterNice.csproj
```

### Clean Build Artifacts
```bash
dotnet clean HarborMasterNice.sln
```

### Build for Release
```bash
dotnet build HarborMasterNice.sln -c Release
```

## Architecture

The application follows the **MVP (Model-View-Presenter)** pattern with a clear separation of concerns:

### Layer Structure (Bottom-Up)

1. **Data Layer** (`Data/`)
   - `SupabaseManager`: Singleton that manages the Supabase client connection
   - Handles authentication (SignIn/SignOut)
   - Connection details: Hard-coded URL and API key (development mode)

2. **Models** (`Models/`)
   - All models inherit from `Postgrest.Models.BaseModel`
   - Use `[Table]` and `[Column]` attributes to map to Supabase tables
   - Core entities: `Ship`, `Berth`, `BerthAssignment`, `Invoice`, `User`
   - `UserRole` defines permission capabilities (e.g., `CanOverrideAllocation()`)

3. **Repositories** (`Repositories/`)
   - `BaseRepository<T, K>`: Generic CRUD operations for all entities
   - Specific repositories extend BaseRepository and add custom queries
   - Example: `BerthRepository.GetPhysicallySuitableBerths()`, `BerthAssignmentRepository.HasScheduleCollision()`
   - All operations are async and return `Task<T>`

4. **Services** (`Services/`)
   - Business logic layer that orchestrates repository calls
   - `AuthenticationService`: User validation and registration (currently uses plain text passwords for development)
   - `PortService`: Core harbor operations - berth allocation logic, schedule conflict detection
   - `NotificationService`: UI notification abstraction

5. **Presenters** (`Presenters/`)
   - Mediates between Views and Services
   - Holds references to View (via interface) and Service instances
   - Contains UI logic flow (loading states, error handling, success callbacks)
   - Example: `LoginPresenter` calls `AuthenticationService.ValidateUser()` then tells View to `HandleLoginSuccess()`

6. **Views** (`Views/`)
   - Windows Forms that implement View interfaces (`ILoginView`, `IMainView`, etc.)
   - Expose properties for Presenter to read (e.g., `Username`, `Password`)
   - Expose methods for Presenter to trigger (e.g., `CloseView()`, `ShowViewAsDialog()`)
   - Handle only UI rendering and user input capture

### Data Flow Example (Login)

```
User clicks Login
  → LoginWindow reads textbox values
  → LoginPresenter.LoginAsync() called
  → AuthenticationService.ValidateUser() queries UserRepository
  → UserRepository uses SupabaseManager.Client to query Supabase
  → Result flows back: Repository → Service → Presenter
  → Presenter calls View.HandleLoginSuccess(user) or View.ErrorMessage = "..."
  → View updates UI accordingly
```

## Key Design Patterns

### MVP Pattern
- **Views** are dumb: They only know how to display data and capture input
- **Presenters** coordinate: They call Services and update Views
- **Views** communicate through interfaces: `ILoginView`, `IMainView`, `IRegisterView`
- This enables testing Presenters without actual UI

### Repository Pattern
- All database access goes through Repositories
- `BaseRepository<T, K>` provides standard CRUD (GetAll, GetById, Insert, Update, Delete)
- Custom queries are added in specific repository classes
- Uses Supabase Postgrest client with async/await

### Service Layer
- Business rules live here (e.g., berth allocation algorithm in `PortService.TryAllocateBerth()`)
- Services are stateless and depend only on Repositories
- Return error messages as strings (empty string = success)

## Important Implementation Details

### Berth Allocation Logic
The core business logic is in `PortService.TryAllocateBerth()`:
1. Validates ETA/ETD times
2. Fetches ship from database
3. Queries physically suitable berths (length/draft requirements)
4. Checks for schedule collisions on each candidate berth
5. If available berth found, creates `BerthAssignment` with status "Scheduled"
6. Respects user role permissions for override capability

### Authentication (Development Mode)
- Passwords are currently stored as **plain text** in the database
- `AuthenticationService` has commented-out BCrypt hashing code
- Password validation uses simple string equality
- This is explicitly noted in code comments as temporary for development

### Async Patterns
- All database operations are async
- Presenters use `async Task` methods
- Views should call Presenter methods with `await` or use `.Wait()` (Windows Forms)
- Use try/catch in Presenters to handle database exceptions

## Database Schema (Supabase)

Tables mapped via attributes:
- `users` → `User` model
- `ships` → `Ship` model
- `berths` → `Berth` model (has `is_available` flag)
- `berth_assignments` → `BerthAssignment` model
- `invoices` → `Invoice` model

Key fields:
- Ships: `imo_number`, `length_overall`, `draft`, `ship_type`
- Berths: `max_length`, `max_draft`, `is_available`
- Assignments: `eta`, `etd`, `actual_arrival_time`, `actual_departure_time`, `status`

## Adding New Features

### To Add a New Entity
1. Create model in `Models/` inheriting `BaseModel` with `[Table]` and `[Column]` attributes
2. Create repository in `Repositories/` extending `BaseRepository<YourModel, int>`
3. Add custom query methods to repository if needed
4. Update or create Service to contain business logic
5. Create View interface in `Views/Interfaces/`
6. Create Presenter in `Presenters/`
7. Create Windows Form implementing the View interface

### To Add a New Business Operation
1. Add method to appropriate Service (or create new Service)
2. Service method should call Repository methods
3. Return error messages as strings (empty = success)
4. Update Presenter to call the new Service method
5. Update View to trigger the Presenter method

## Common Patterns to Follow

### Error Handling
```csharp
try
{
    _view.IsLoading = true;
    var result = await _service.DoSomethingAsync();
    if (string.IsNullOrEmpty(result))
    {
        // Success
        _view.SuccessMessage = "Done!";
    }
    else
    {
        // Business logic error
        _view.ErrorMessage = result;
    }
}
catch (Exception ex)
{
    // System error
    _view.ErrorMessage = $"Error: {ex.Message}";
}
finally
{
    _view.IsLoading = false;
}
```

### Repository Custom Query
```csharp
public async Task<List<Ship>> GetShipsByType(string type)
{
    var response = await _client.From<Ship>()
        .Filter("ship_type", Postgrest.Constants.Operator.Equals, type)
        .Get();
    return response.Models;
}
```

## Dependencies

- .NET 8.0 Windows Forms
- `supabase-csharp` (v0.16.2) - Supabase client library
- Postgrest (transitive dependency)

## Development Environment

- Visual Studio 2022 (or compatible IDE)
- Windows OS required (Windows Forms)
- No test project currently exists
