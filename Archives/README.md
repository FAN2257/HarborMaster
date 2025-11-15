# Archives

This folder contains archived implementations of the HarborMaster project for reference purposes.

## Folder Structure

### `v1-simple/` - Simple Windows Forms Implementation
**Date:** November 2024  
**Description:** Simple monolithic Windows Forms application demonstrating OOP concepts.

**Characteristics:**
- **Architecture:** Monolithic (UI + Logic mixed)
- **Data Storage:** Static in-memory lists
- **Persistence:** None (data lost on app close)
- **Purpose:** OOP demonstration (Inheritance, Polymorphism, Encapsulation)

**Key Files:**
- `HarborMaster/Ship.cs` - Abstract Ship class with derived classes
- `HarborMaster/PortService.cs` - Service with static data storage
- `HarborMaster/MainForm.cs` - Windows Forms UI

**Use Case:** Learning OOP concepts, quick prototyping

---

### `v0-oldest/` - Original Implementation
**Date:** October 2024  
**Description:** Earliest version of the HarborMaster project.

**Purpose:** Historical reference

---

## Active Implementation

The current **production-ready** implementation is in the root directory using:
- **Architecture:** MVP (Model-View-Presenter) pattern
- **Data Storage:** PostgreSQL via Supabase
- **Persistence:** Full database persistence
- **Layers:** 6-layer architecture (Views, Presenters, Services, Repositories, Models, Data)

See root `README.md` for details on the active implementation.

---

## Migration History

```
v0-oldest (2024-10)
    ↓
v1-simple (2024-11) - Added OOP concepts
    ↓
Current MVP (2024-11) - Production-ready with database
```

---

**Note:** Archived implementations are kept for:
- Reference and learning
- Understanding architectural evolution
- Code examples for OOP patterns

They are **not maintained** and may not build with current dependencies.
