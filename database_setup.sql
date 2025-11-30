-- =====================================================
-- HarborMasterNice Database Setup Script
-- PostgreSQL/Supabase Database Schema
-- =====================================================
-- Version: 1.0
-- Description: Complete database schema for harbor management system
-- Features: User roles, ship management, berth allocation, invoicing
-- =====================================================

-- =====================================================
-- 1. DROP TABLES & TYPES (untuk clean setup)
-- =====================================================
-- Hati-hati: Script ini akan menghapus semua data!
-- Uncomment jika ingin clean install

-- -- Drop tables first (order matters due to foreign keys)
-- DROP TABLE IF EXISTS invoices CASCADE;
-- DROP TABLE IF EXISTS docking_requests CASCADE;
-- DROP TABLE IF EXISTS berth_assignments CASCADE;
-- DROP TABLE IF EXISTS ships CASCADE;
-- DROP TABLE IF EXISTS berths CASCADE;
-- DROP TABLE IF EXISTS ship_type_multipliers CASCADE;
-- DROP TABLE IF EXISTS size_multipliers CASCADE;
-- DROP TABLE IF EXISTS users CASCADE;
--
-- -- Drop custom types after tables
-- DROP TYPE IF EXISTS user_role;

-- =====================================================
-- 2. CREATE ENUM TYPES
-- =====================================================

-- Create user_role enum type if it doesn't exist
DO $$ BEGIN
    CREATE TYPE user_role AS ENUM ('ShipOwner', 'Operator', 'HarborMaster');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- =====================================================
-- 3. CREATE TABLES
-- =====================================================

-- -----------------------------------------------------
-- Table: users
-- Description: User accounts with role-based access
-- Roles: ShipOwner, Operator, HarborMaster
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    full_name VARCHAR(100) NOT NULL,
    role user_role NOT NULL DEFAULT 'ShipOwner',
    password_hash VARCHAR(255),
    email VARCHAR(100),
    phone VARCHAR(20),
    company_name VARCHAR(150),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    -- Constraints
    CONSTRAINT chk_username_length CHECK (LENGTH(username) >= 3),
    CONSTRAINT chk_email_format CHECK (email IS NULL OR email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$')
);

-- Create index for faster username lookups
CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);
CREATE INDEX IF NOT EXISTS idx_users_role ON users(role);

COMMENT ON TABLE users IS 'User accounts with role-based permissions';
COMMENT ON COLUMN users.role IS 'ShipOwner, Operator, or HarborMaster';
COMMENT ON COLUMN users.password_hash IS 'Currently plain text in development (will be BCrypt hashed in production)';

-- -----------------------------------------------------
-- Table: ships
-- Description: Ship registry with owner relationship
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS ships (
    id SERIAL PRIMARY KEY,
    owner_id INTEGER,
    name VARCHAR(100) NOT NULL,
    imo_number VARCHAR(20),
    length_overall NUMERIC(8,2) NOT NULL,
    draft NUMERIC(6,2) NOT NULL,
    ship_type VARCHAR(50) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    -- Foreign Keys
    CONSTRAINT fk_ship_owner FOREIGN KEY (owner_id)
        REFERENCES users(id) ON DELETE SET NULL,

    -- Constraints
    CONSTRAINT chk_ship_type CHECK (
        ship_type IN ('Container', 'Tanker', 'Bulk Carrier', 'General Cargo', 'Passenger')
    ),
    CONSTRAINT chk_length_positive CHECK (length_overall > 0 AND length_overall <= 500),
    CONSTRAINT chk_draft_positive CHECK (draft > 0 AND draft <= 30),
    CONSTRAINT chk_name_not_empty CHECK (LENGTH(TRIM(name)) > 0)
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_ships_owner_id ON ships(owner_id);
CREATE INDEX IF NOT EXISTS idx_ships_ship_type ON ships(ship_type);
CREATE INDEX IF NOT EXISTS idx_ships_imo_number ON ships(imo_number);

COMMENT ON TABLE ships IS 'Ship registry with dimensions and type information';
COMMENT ON COLUMN ships.owner_id IS 'References users table - can be NULL for legacy ships';
COMMENT ON COLUMN ships.imo_number IS 'International Maritime Organization number (optional)';
COMMENT ON COLUMN ships.length_overall IS 'Total ship length in meters';
COMMENT ON COLUMN ships.draft IS 'Ship draft in meters (depth below waterline)';

-- -----------------------------------------------------
-- Table: berths
-- Description: Berth/dock registry with capacity info
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS berths (
    id SERIAL PRIMARY KEY,
    berth_name VARCHAR(50) NOT NULL UNIQUE,
    location VARCHAR(100) NOT NULL,
    max_length NUMERIC(8,2) NOT NULL,
    max_draft NUMERIC(6,2) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Available',
    base_rate_per_day NUMERIC(12,2) NOT NULL DEFAULT 1000.00,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    -- Constraints
    CONSTRAINT chk_berth_status CHECK (
        status IN ('Available', 'Occupied', 'Maintenance', 'Damaged')
    ),
    CONSTRAINT chk_berth_max_length CHECK (max_length > 0),
    CONSTRAINT chk_berth_max_draft CHECK (max_draft > 0),
    CONSTRAINT chk_berth_rate CHECK (base_rate_per_day >= 0)
);

-- Indexes
CREATE INDEX IF NOT EXISTS idx_berths_status ON berths(status);
CREATE INDEX IF NOT EXISTS idx_berths_name ON berths(berth_name);

COMMENT ON TABLE berths IS 'Berth/dock locations with capacity and status';
COMMENT ON COLUMN berths.status IS 'Available, Occupied, Maintenance, or Damaged';
COMMENT ON COLUMN berths.base_rate_per_day IS 'Base daily rate in currency units';

-- -----------------------------------------------------
-- Table: berth_assignments
-- Description: Ship docking schedule and assignments
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS berth_assignments (
    id SERIAL PRIMARY KEY,
    ship_id INTEGER NOT NULL,
    berth_id INTEGER NOT NULL,
    eta TIMESTAMPTZ NOT NULL,
    etd TIMESTAMPTZ NOT NULL,
    actual_arrival_time TIMESTAMPTZ,
    actual_departure_time TIMESTAMPTZ,
    status VARCHAR(20) NOT NULL DEFAULT 'Scheduled',
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    -- Foreign Keys
    CONSTRAINT fk_assignment_ship FOREIGN KEY (ship_id)
        REFERENCES ships(id) ON DELETE CASCADE,
    CONSTRAINT fk_assignment_berth FOREIGN KEY (berth_id)
        REFERENCES berths(id) ON DELETE CASCADE,

    -- Constraints
    CONSTRAINT chk_assignment_status CHECK (
        status IN ('Scheduled', 'Arrived', 'Departed', 'Delayed', 'Cancelled')
    ),
    CONSTRAINT chk_eta_before_etd CHECK (eta < etd),
    CONSTRAINT chk_actual_arrival_before_departure CHECK (
        actual_arrival_time IS NULL OR
        actual_departure_time IS NULL OR
        actual_arrival_time < actual_departure_time
    )
);

-- Indexes for schedule queries
CREATE INDEX IF NOT EXISTS idx_assignments_ship_id ON berth_assignments(ship_id);
CREATE INDEX IF NOT EXISTS idx_assignments_berth_id ON berth_assignments(berth_id);
CREATE INDEX IF NOT EXISTS idx_assignments_eta ON berth_assignments(eta);
CREATE INDEX IF NOT EXISTS idx_assignments_etd ON berth_assignments(etd);
CREATE INDEX IF NOT EXISTS idx_assignments_status ON berth_assignments(status);
CREATE INDEX IF NOT EXISTS idx_assignments_berth_time ON berth_assignments(berth_id, eta, etd);

COMMENT ON TABLE berth_assignments IS 'Ship docking schedule and allocation records';
COMMENT ON COLUMN berth_assignments.status IS 'Scheduled, Arrived, Departed, Delayed, or Cancelled';

-- -----------------------------------------------------
-- Table: docking_requests
-- Description: Ship owner requests for docking approval
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS docking_requests (
    id SERIAL PRIMARY KEY,
    ship_id INTEGER NOT NULL,
    owner_id INTEGER NOT NULL,
    requested_eta TIMESTAMPTZ NOT NULL,
    requested_etd TIMESTAMPTZ NOT NULL,
    cargo_type VARCHAR(100),
    special_requirements TEXT,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    created_at TIMESTAMPTZ DEFAULT NOW(),
    processed_by INTEGER,
    processed_at TIMESTAMPTZ,
    rejection_reason TEXT,
    berth_assignment_id INTEGER,

    -- Foreign Keys
    CONSTRAINT fk_request_ship FOREIGN KEY (ship_id)
        REFERENCES ships(id) ON DELETE CASCADE,
    CONSTRAINT fk_request_owner FOREIGN KEY (owner_id)
        REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_request_processed_by FOREIGN KEY (processed_by)
        REFERENCES users(id) ON DELETE SET NULL,
    CONSTRAINT fk_request_assignment FOREIGN KEY (berth_assignment_id)
        REFERENCES berth_assignments(id) ON DELETE SET NULL,

    -- Constraints
    CONSTRAINT chk_request_status CHECK (
        status IN ('Pending', 'Approved', 'Rejected', 'Cancelled')
    ),
    CONSTRAINT chk_request_eta_before_etd CHECK (requested_eta < requested_etd),
    CONSTRAINT chk_request_duration CHECK (
        EXTRACT(EPOCH FROM (requested_etd - requested_eta))/86400 <= 90
    )
);

-- Indexes
CREATE INDEX IF NOT EXISTS idx_requests_ship_id ON docking_requests(ship_id);
CREATE INDEX IF NOT EXISTS idx_requests_owner_id ON docking_requests(owner_id);
CREATE INDEX IF NOT EXISTS idx_requests_status ON docking_requests(status);
CREATE INDEX IF NOT EXISTS idx_requests_created_at ON docking_requests(created_at);

COMMENT ON TABLE docking_requests IS 'Docking requests submitted by ship owners';
COMMENT ON COLUMN docking_requests.status IS 'Pending, Approved, Rejected, or Cancelled';

-- -----------------------------------------------------
-- Table: invoices
-- Description: Billing and payment tracking
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS invoices (
    id SERIAL PRIMARY KEY,
    berth_assignment_id INTEGER NOT NULL,
    total_amount NUMERIC(15,2) NOT NULL,
    issued_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    due_date TIMESTAMPTZ NOT NULL,
    is_paid BOOLEAN NOT NULL DEFAULT FALSE,
    paid_at TIMESTAMPTZ,
    notes TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    -- Foreign Keys
    CONSTRAINT fk_invoice_assignment FOREIGN KEY (berth_assignment_id)
        REFERENCES berth_assignments(id) ON DELETE CASCADE,

    -- Constraints
    CONSTRAINT chk_total_amount_positive CHECK (total_amount >= 0),
    CONSTRAINT chk_due_date_after_issued CHECK (due_date >= issued_date)
);

-- Indexes
CREATE INDEX IF NOT EXISTS idx_invoices_assignment ON invoices(berth_assignment_id);
CREATE INDEX IF NOT EXISTS idx_invoices_is_paid ON invoices(is_paid);
CREATE INDEX IF NOT EXISTS idx_invoices_due_date ON invoices(due_date);

COMMENT ON TABLE invoices IS 'Billing records for berth assignments';
COMMENT ON COLUMN invoices.is_paid IS 'Payment status flag';

-- -----------------------------------------------------
-- Table: ship_type_multipliers
-- Description: Pricing multipliers by ship type
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS ship_type_multipliers (
    id SERIAL PRIMARY KEY,
    ship_type VARCHAR(50) NOT NULL UNIQUE,
    multiplier NUMERIC(5,2) NOT NULL,
    description TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    -- Constraints
    CONSTRAINT chk_type_multiplier_positive CHECK (multiplier > 0),
    CONSTRAINT chk_ship_type_valid CHECK (
        ship_type IN ('Container', 'Tanker', 'Bulk Carrier', 'General Cargo', 'Passenger')
    )
);

COMMENT ON TABLE ship_type_multipliers IS 'Pricing multipliers based on ship type';

-- -----------------------------------------------------
-- Table: size_multipliers
-- Description: Pricing multipliers by ship size
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS size_multipliers (
    id SERIAL PRIMARY KEY,
    min_length NUMERIC(8,2) NOT NULL,
    max_length NUMERIC(8,2) NOT NULL,
    multiplier NUMERIC(5,2) NOT NULL,
    category VARCHAR(50) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    -- Constraints
    CONSTRAINT chk_size_range CHECK (min_length < max_length),
    CONSTRAINT chk_size_multiplier_positive CHECK (multiplier > 0)
);

CREATE INDEX IF NOT EXISTS idx_size_multipliers_range ON size_multipliers(min_length, max_length);

COMMENT ON TABLE size_multipliers IS 'Pricing multipliers based on ship size ranges';

-- =====================================================
-- 3. TRIGGERS untuk auto-update timestamps
-- =====================================================

-- Function untuk update timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Apply trigger ke semua tabel yang punya updated_at
DROP TRIGGER IF EXISTS update_users_updated_at ON users;
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_ships_updated_at ON ships;
CREATE TRIGGER update_ships_updated_at BEFORE UPDATE ON ships
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_berths_updated_at ON berths;
CREATE TRIGGER update_berths_updated_at BEFORE UPDATE ON berths
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_berth_assignments_updated_at ON berth_assignments;
CREATE TRIGGER update_berth_assignments_updated_at BEFORE UPDATE ON berth_assignments
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_invoices_updated_at ON invoices;
CREATE TRIGGER update_invoices_updated_at BEFORE UPDATE ON invoices
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_ship_type_multipliers_updated_at ON ship_type_multipliers;
CREATE TRIGGER update_ship_type_multipliers_updated_at BEFORE UPDATE ON ship_type_multipliers
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_size_multipliers_updated_at ON size_multipliers;
CREATE TRIGGER update_size_multipliers_updated_at BEFORE UPDATE ON size_multipliers
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- 4. SAMPLE DATA / INITIAL DATA
-- =====================================================

-- -----------------------------------------------------
-- Insert Users (Development Mode - Plain Text Passwords)
-- -----------------------------------------------------
-- Only insert if users don't already exist
INSERT INTO users (username, full_name, role, password_hash, email, phone, company_name)
SELECT 'harbormaster', 'John Harbor', 'HarborMaster'::user_role, 'admin123', 'john.harbor@harbormaster.com', '+62-811-1111-1111', 'Harbor Authority'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'harbormaster');

INSERT INTO users (username, full_name, role, password_hash, email, phone, company_name)
SELECT 'operator1', 'Sarah Operations', 'Operator'::user_role, 'operator123', 'sarah.ops@harbormaster.com', '+62-811-2222-2222', 'Harbor Authority'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'operator1');

INSERT INTO users (username, full_name, role, password_hash, email, phone, company_name)
SELECT 'operator2', 'Mike Manager', 'Operator'::user_role, 'operator123', 'mike.manager@harbormaster.com', '+62-811-3333-3333', 'Harbor Authority'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'operator2');

INSERT INTO users (username, full_name, role, password_hash, email, phone, company_name)
SELECT 'shipowner1', 'David Shipping', 'ShipOwner'::user_role, 'owner123', 'david@shipping-corp.com', '+62-811-4444-4444', 'Global Shipping Corp'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'shipowner1');

INSERT INTO users (username, full_name, role, password_hash, email, phone, company_name)
SELECT 'shipowner2', 'Emma Maritime', 'ShipOwner'::user_role, 'owner123', 'emma@maritime-lines.com', '+62-811-5555-5555', 'Maritime Lines Ltd'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'shipowner2');

INSERT INTO users (username, full_name, role, password_hash, email, phone, company_name)
SELECT 'shipowner3', 'Robert Tanker', 'ShipOwner'::user_role, 'owner123', 'robert@oceantankers.com', '+62-811-6666-6666', 'Ocean Tankers Inc'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'shipowner3');

-- -----------------------------------------------------
-- Insert Ship Type Multipliers (Pricing Config)
-- -----------------------------------------------------
INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Container', 1.20, 'Container ships require specialized crane services'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Container');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Tanker', 1.50, 'Tankers need hazmat handling and safety measures'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Tanker');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Bulk Carrier', 1.10, 'Bulk carriers require bulk loading equipment'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Bulk Carrier');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'General Cargo', 1.00, 'Standard cargo handling services'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'General Cargo');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Passenger', 1.40, 'Passenger ships need terminal and customs services'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Passenger');

-- -----------------------------------------------------
-- Insert Size Multipliers (Pricing Config)
-- -----------------------------------------------------
INSERT INTO size_multipliers (min_length, max_length, multiplier, category) VALUES
(0, 50, 0.80, 'Small Vessel'),
(50, 100, 1.00, 'Medium Vessel'),
(100, 200, 1.30, 'Large Vessel'),
(200, 300, 1.60, 'Very Large Vessel'),
(300, 500, 2.00, 'Ultra Large Vessel');

-- -----------------------------------------------------
-- Insert Berths
-- -----------------------------------------------------
INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day) VALUES
('Berth A1', 'North Dock - Section A', 150.00, 12.00, 'Available', 5000.00),
('Berth A2', 'North Dock - Section A', 180.00, 14.00, 'Available', 6000.00),
('Berth A3', 'North Dock - Section A', 200.00, 15.00, 'Available', 7000.00),
('Berth B1', 'South Dock - Section B', 250.00, 18.00, 'Available', 10000.00),
('Berth B2', 'South Dock - Section B', 300.00, 20.00, 'Available', 12000.00),
('Berth B3', 'South Dock - Section B', 300.00, 20.00, 'Maintenance', 12000.00),
('Berth C1', 'East Dock - Section C', 120.00, 10.00, 'Available', 4000.00),
('Berth C2', 'East Dock - Section C', 160.00, 12.00, 'Available', 5500.00),
('Berth D1', 'West Dock - Section D (Tanker)', 280.00, 22.00, 'Available', 15000.00),
('Berth D2', 'West Dock - Section D (Tanker)', 320.00, 25.00, 'Available', 18000.00)
ON CONFLICT (berth_name) DO NOTHING;

-- -----------------------------------------------------
-- Insert Sample Ships (owned by ship owners)
-- -----------------------------------------------------
INSERT INTO ships (owner_id, name, imo_number, length_overall, draft, ship_type) VALUES
-- David Shipping's ships (owner_id = 4)
(4, 'MV Pacific Glory', 'IMO9234567', 180.50, 12.30, 'Container'),
(4, 'MV Atlantic Star', 'IMO9234568', 220.00, 14.50, 'Container'),
(4, 'MV Cargo Express', 'IMO9234569', 145.00, 10.20, 'General Cargo'),

-- Emma Maritime's ships (owner_id = 5)
(5, 'MV Ocean Breeze', 'IMO9234570', 195.80, 13.50, 'Bulk Carrier'),
(5, 'MV Maritime Prince', 'IMO9234571', 250.00, 16.00, 'Bulk Carrier'),
(5, 'MV Coastal Trader', 'IMO9234572', 130.00, 9.50, 'General Cargo'),

-- Robert Tanker's ships (owner_id = 6)
(6, 'MT Ocean Voyager', 'IMO9234573', 280.00, 20.00, 'Tanker'),
(6, 'MT Crude Carrier', 'IMO9234574', 310.00, 22.50, 'Tanker'),

-- Legacy ships (no owner)
(NULL, 'MV Cruise Paradise', 'IMO9234575', 290.00, 18.00, 'Passenger'),
(NULL, 'MV Island Hopper', 'IMO9234576', 160.00, 11.00, 'Passenger');

-- -----------------------------------------------------
-- Insert Berth Assignments (Some scheduled, some arrived)
-- -----------------------------------------------------
INSERT INTO berth_assignments (ship_id, berth_id, eta, etd, actual_arrival_time, actual_departure_time, status) VALUES
-- Currently docked ships (Arrived status)
(1, 1, NOW() - INTERVAL '2 days', NOW() + INTERVAL '3 days', NOW() - INTERVAL '2 days', NULL, 'Arrived'),
(4, 4, NOW() - INTERVAL '1 day', NOW() + INTERVAL '5 days', NOW() - INTERVAL '1 day', NULL, 'Arrived'),
(7, 9, NOW() - INTERVAL '3 days', NOW() + INTERVAL '2 days', NOW() - INTERVAL '3 days', NULL, 'Arrived'),

-- Scheduled for future
(2, 2, NOW() + INTERVAL '2 days', NOW() + INTERVAL '8 days', NULL, NULL, 'Scheduled'),
(3, 7, NOW() + INTERVAL '3 days', NOW() + INTERVAL '7 days', NULL, NULL, 'Scheduled'),
(5, 5, NOW() + INTERVAL '5 days', NOW() + INTERVAL '12 days', NULL, NULL, 'Scheduled'),
(8, 10, NOW() + INTERVAL '4 days', NOW() + INTERVAL '10 days', NULL, NULL, 'Scheduled'),

-- Departed (historical)
(6, 8, NOW() - INTERVAL '10 days', NOW() - INTERVAL '5 days', NOW() - INTERVAL '10 days', NOW() - INTERVAL '5 days', 'Departed'),
(9, 1, NOW() - INTERVAL '15 days', NOW() - INTERVAL '12 days', NOW() - INTERVAL '15 days', NOW() - INTERVAL '12 days', 'Departed'),
(10, 2, NOW() - INTERVAL '20 days', NOW() - INTERVAL '17 days', NOW() - INTERVAL '20 days', NOW() - INTERVAL '17 days', 'Departed');

-- -----------------------------------------------------
-- Insert Docking Requests (Various statuses)
-- -----------------------------------------------------
INSERT INTO docking_requests (ship_id, owner_id, requested_eta, requested_etd, cargo_type, special_requirements, status, processed_by, processed_at, berth_assignment_id) VALUES
-- Approved requests (linked to assignments)
(1, 4, NOW() - INTERVAL '2 days', NOW() + INTERVAL '3 days', 'Electronics', 'Need crane service', 'Approved', 2, NOW() - INTERVAL '3 days', 1),
(2, 4, NOW() + INTERVAL '2 days', NOW() + INTERVAL '8 days', 'Textiles', NULL, 'Approved', 2, NOW() - INTERVAL '1 day', 4),
(7, 6, NOW() - INTERVAL '3 days', NOW() + INTERVAL '2 days', 'Crude Oil', 'Hazmat team required', 'Approved', 1, NOW() - INTERVAL '5 days', 3),

-- Pending requests
(3, 4, NOW() + INTERVAL '10 days', NOW() + INTERVAL '15 days', 'Machinery', NULL, 'Pending', NULL, NULL, NULL),
(5, 5, NOW() + INTERVAL '12 days', NOW() + INTERVAL '18 days', 'Iron Ore', 'Heavy duty equipment needed', 'Pending', NULL, NULL, NULL),

-- Rejected request
(6, 5, NOW() + INTERVAL '2 days', NOW() + INTERVAL '4 days', 'Coal', NULL, 'Rejected', 2, NOW() - INTERVAL '1 day', NULL);

-- Update rejection reason
UPDATE docking_requests
SET rejection_reason = 'Time slot conflicts with scheduled maintenance'
WHERE status = 'Rejected';

-- -----------------------------------------------------
-- Insert Invoices for completed/ongoing assignments
-- -----------------------------------------------------
INSERT INTO invoices (berth_assignment_id, total_amount, issued_date, due_date, is_paid, paid_at, notes) VALUES
-- Paid invoice (for departed ship)
(8, 32500.00, NOW() - INTERVAL '10 days', NOW() - INTERVAL '3 days', TRUE, NOW() - INTERVAL '4 days', 'Payment received via bank transfer'),

-- Unpaid invoices (for current assignments)
(1, 42000.00, NOW() - INTERVAL '1 day', NOW() + INTERVAL '14 days', FALSE, NULL, 'Invoice sent to Global Shipping Corp'),
(2, 68000.00, NOW(), NOW() + INTERVAL '15 days', FALSE, NULL, 'Invoice pending'),

-- Overdue invoice
(9, 18500.00, NOW() - INTERVAL '12 days', NOW() - INTERVAL '2 days', FALSE, NULL, 'Payment overdue - follow up required');

-- =====================================================
-- 5. VIEWS untuk Query yang Sering Digunakan
-- =====================================================

-- View: Current Berth Occupancy
CREATE OR REPLACE VIEW vw_current_berth_occupancy AS
SELECT
    b.id,
    b.berth_name,
    b.location,
    b.status,
    b.max_length,
    b.max_draft,
    ba.id as assignment_id,
    s.name as ship_name,
    s.ship_type,
    ba.eta,
    ba.etd,
    ba.status as assignment_status
FROM berths b
LEFT JOIN berth_assignments ba ON b.id = ba.berth_id
    AND ba.status IN ('Scheduled', 'Arrived')
    AND NOW() BETWEEN ba.eta AND ba.etd
LEFT JOIN ships s ON ba.ship_id = s.id
ORDER BY b.berth_name;

-- View: Pending Docking Requests Summary
CREATE OR REPLACE VIEW vw_pending_requests AS
SELECT
    dr.id,
    dr.requested_eta,
    dr.requested_etd,
    dr.cargo_type,
    dr.special_requirements,
    dr.created_at,
    s.name as ship_name,
    s.ship_type,
    s.length_overall,
    s.draft,
    u.full_name as owner_name,
    u.company_name
FROM docking_requests dr
JOIN ships s ON dr.ship_id = s.id
JOIN users u ON dr.owner_id = u.id
WHERE dr.status = 'Pending'
ORDER BY dr.created_at ASC;

-- View: Revenue Summary
CREATE OR REPLACE VIEW vw_revenue_summary AS
SELECT
    DATE_TRUNC('month', i.issued_date) as month,
    COUNT(*) as total_invoices,
    SUM(i.total_amount) as total_revenue,
    SUM(CASE WHEN i.is_paid THEN i.total_amount ELSE 0 END) as paid_amount,
    SUM(CASE WHEN NOT i.is_paid THEN i.total_amount ELSE 0 END) as unpaid_amount,
    COUNT(CASE WHEN i.is_paid THEN 1 END) as paid_invoices,
    COUNT(CASE WHEN NOT i.is_paid THEN 1 END) as unpaid_invoices
FROM invoices i
GROUP BY DATE_TRUNC('month', i.issued_date)
ORDER BY month DESC;

-- =====================================================
-- 6. FUNCTIONS untuk Business Logic
-- =====================================================

-- Function: Check Schedule Collision
CREATE OR REPLACE FUNCTION fn_check_schedule_collision(
    p_berth_id INTEGER,
    p_eta TIMESTAMPTZ,
    p_etd TIMESTAMPTZ,
    p_exclude_assignment_id INTEGER DEFAULT NULL
)
RETURNS BOOLEAN AS $$
DECLARE
    v_collision_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO v_collision_count
    FROM berth_assignments
    WHERE berth_id = p_berth_id
        AND status IN ('Scheduled', 'Arrived')
        AND (p_exclude_assignment_id IS NULL OR id != p_exclude_assignment_id)
        AND (
            (eta <= p_eta AND etd > p_eta) OR
            (eta < p_etd AND etd >= p_etd) OR
            (eta >= p_eta AND etd <= p_etd)
        );

    RETURN v_collision_count > 0;
END;
$$ LANGUAGE plpgsql;

-- Function: Get Available Berths for Ship
CREATE OR REPLACE FUNCTION fn_get_available_berths(
    p_ship_id INTEGER,
    p_eta TIMESTAMPTZ,
    p_etd TIMESTAMPTZ
)
RETURNS TABLE (
    berth_id INTEGER,
    berth_name VARCHAR,
    location VARCHAR,
    base_rate_per_day NUMERIC
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        b.id,
        b.berth_name,
        b.location,
        b.base_rate_per_day
    FROM berths b
    JOIN ships s ON s.id = p_ship_id
    WHERE b.status = 'Available'
        AND s.length_overall <= b.max_length
        AND s.draft <= b.max_draft
        AND NOT fn_check_schedule_collision(b.id, p_eta, p_etd)
    ORDER BY b.base_rate_per_day ASC;
END;
$$ LANGUAGE plpgsql;

-- Function: Calculate Invoice Amount
CREATE OR REPLACE FUNCTION fn_calculate_invoice_amount(
    p_berth_assignment_id INTEGER
)
RETURNS NUMERIC AS $$
DECLARE
    v_days INTEGER;
    v_base_rate NUMERIC;
    v_ship_type VARCHAR;
    v_ship_length NUMERIC;
    v_type_multiplier NUMERIC;
    v_size_multiplier NUMERIC;
    v_total NUMERIC;
BEGIN
    -- Get assignment details
    SELECT
        EXTRACT(EPOCH FROM (ba.etd - ba.eta))/86400,
        b.base_rate_per_day,
        s.ship_type,
        s.length_overall
    INTO v_days, v_base_rate, v_ship_type, v_ship_length
    FROM berth_assignments ba
    JOIN berths b ON ba.berth_id = b.id
    JOIN ships s ON ba.ship_id = s.id
    WHERE ba.id = p_berth_assignment_id;

    -- Get multipliers
    SELECT multiplier INTO v_type_multiplier
    FROM ship_type_multipliers
    WHERE ship_type = v_ship_type;

    SELECT multiplier INTO v_size_multiplier
    FROM size_multipliers
    WHERE v_ship_length >= min_length AND v_ship_length < max_length
    LIMIT 1;

    -- Calculate total
    v_total := v_days * v_base_rate *
               COALESCE(v_type_multiplier, 1.0) *
               COALESCE(v_size_multiplier, 1.0);

    RETURN ROUND(v_total, 2);
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- 7. GRANT PERMISSIONS (Optional - for Supabase RLS)
-- =====================================================
-- Uncomment jika menggunakan Row Level Security di Supabase

-- ALTER TABLE users ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE ships ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE berth_assignments ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE docking_requests ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE invoices ENABLE ROW LEVEL SECURITY;

-- CREATE POLICY "Users can view their own data" ON users
--     FOR SELECT USING (auth.uid() = id);

-- CREATE POLICY "Ship owners can view their own ships" ON ships
--     FOR SELECT USING (owner_id = auth.uid());

-- =====================================================
-- SETUP COMPLETE!
-- =====================================================
-- Script telah berhasil membuat:
-- ✅ 8 Tables dengan constraints lengkap
-- ✅ Indexes untuk performance optimization
-- ✅ Foreign key relationships
-- ✅ Auto-update triggers untuk timestamps
-- ✅ Sample data untuk testing
-- ✅ Useful views untuk reporting
-- ✅ Business logic functions
--
-- Untuk menjalankan script ini:
-- 1. Buka Supabase SQL Editor atau pgAdmin
-- 2. Copy-paste seluruh script ini
-- 3. Execute
--
-- Users untuk testing:
-- - HarborMaster: harbormaster / admin123
-- - Operator: operator1 / operator123
-- - Ship Owner: shipowner1 / owner123
-- =====================================================
