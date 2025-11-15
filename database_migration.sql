-- =====================================================
-- HarborMasterNice Database Migration Script
-- Menyesuaikan struktur tabel yang sudah ada di Supabase
-- =====================================================

-- =====================================================
-- 1. USERS TABLE
-- =====================================================
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    full_name VARCHAR(100) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('Operator', 'HarborMaster')),
    password_hash VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tambahkan kolom jika belum ada
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='users' AND column_name='username') THEN
        ALTER TABLE users ADD COLUMN username VARCHAR(50) NOT NULL UNIQUE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='users' AND column_name='full_name') THEN
        ALTER TABLE users ADD COLUMN full_name VARCHAR(100) NOT NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='users' AND column_name='role') THEN
        ALTER TABLE users ADD COLUMN role VARCHAR(20) NOT NULL CHECK (role IN ('Operator', 'HarborMaster'));
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='users' AND column_name='password_hash') THEN
        ALTER TABLE users ADD COLUMN password_hash VARCHAR(255);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='users' AND column_name='created_at') THEN
        ALTER TABLE users ADD COLUMN created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='users' AND column_name='updated_at') THEN
        ALTER TABLE users ADD COLUMN updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;
END $$;

-- Index untuk username
CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);

-- =====================================================
-- 2. SHIPS TABLE
-- =====================================================
CREATE TABLE IF NOT EXISTS ships (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    imo_number VARCHAR(20),
    length_overall DECIMAL(6,2) NOT NULL CHECK (length_overall > 0 AND length_overall <= 500),
    draft DECIMAL(5,2) NOT NULL CHECK (draft > 0 AND draft <= 30),
    ship_type VARCHAR(50) NOT NULL CHECK (ship_type IN ('Container', 'Tanker', 'Bulk Carrier', 'General Cargo', 'Passenger')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tambahkan kolom jika belum ada
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ships' AND column_name='name') THEN
        ALTER TABLE ships ADD COLUMN name VARCHAR(100) NOT NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ships' AND column_name='imo_number') THEN
        ALTER TABLE ships ADD COLUMN imo_number VARCHAR(20);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ships' AND column_name='length_overall') THEN
        ALTER TABLE ships ADD COLUMN length_overall DECIMAL(6,2) NOT NULL CHECK (length_overall > 0 AND length_overall <= 500);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ships' AND column_name='draft') THEN
        ALTER TABLE ships ADD COLUMN draft DECIMAL(5,2) NOT NULL CHECK (draft > 0 AND draft <= 30);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ships' AND column_name='ship_type') THEN
        ALTER TABLE ships ADD COLUMN ship_type VARCHAR(50) NOT NULL CHECK (ship_type IN ('Container', 'Tanker', 'Bulk Carrier', 'General Cargo', 'Passenger'));
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ships' AND column_name='created_at') THEN
        ALTER TABLE ships ADD COLUMN created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ships' AND column_name='updated_at') THEN
        ALTER TABLE ships ADD COLUMN updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;
END $$;

-- Indexes
CREATE INDEX IF NOT EXISTS idx_ships_imo ON ships(imo_number);
CREATE INDEX IF NOT EXISTS idx_ships_type ON ships(ship_type);
CREATE INDEX IF NOT EXISTS idx_ships_name ON ships(name);

-- =====================================================
-- 3. BERTHS TABLE
-- =====================================================
CREATE TABLE IF NOT EXISTS berths (
    id SERIAL PRIMARY KEY,
    berth_name VARCHAR(50) NOT NULL UNIQUE,
    location VARCHAR(100) NOT NULL,
    max_length DECIMAL(6,2) NOT NULL CHECK (max_length > 0),
    max_draft DECIMAL(5,2) NOT NULL CHECK (max_draft > 0),
    is_available BOOLEAN DEFAULT TRUE,
    base_rate_per_day DECIMAL(10,2) NOT NULL CHECK (base_rate_per_day >= 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tambahkan kolom jika belum ada
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berths' AND column_name='berth_name') THEN
        ALTER TABLE berths ADD COLUMN berth_name VARCHAR(50) NOT NULL UNIQUE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berths' AND column_name='location') THEN
        ALTER TABLE berths ADD COLUMN location VARCHAR(100) NOT NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berths' AND column_name='max_length') THEN
        ALTER TABLE berths ADD COLUMN max_length DECIMAL(6,2) NOT NULL CHECK (max_length > 0);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berths' AND column_name='max_draft') THEN
        ALTER TABLE berths ADD COLUMN max_draft DECIMAL(5,2) NOT NULL CHECK (max_draft > 0);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berths' AND column_name='is_available') THEN
        ALTER TABLE berths ADD COLUMN is_available BOOLEAN DEFAULT TRUE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berths' AND column_name='base_rate_per_day') THEN
        ALTER TABLE berths ADD COLUMN base_rate_per_day DECIMAL(10,2) NOT NULL DEFAULT 0 CHECK (base_rate_per_day >= 0);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berths' AND column_name='created_at') THEN
        ALTER TABLE berths ADD COLUMN created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berths' AND column_name='updated_at') THEN
        ALTER TABLE berths ADD COLUMN updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;
END $$;

-- Indexes
CREATE INDEX IF NOT EXISTS idx_berths_available ON berths(is_available);
CREATE INDEX IF NOT EXISTS idx_berths_capacity ON berths(max_length, max_draft);

-- =====================================================
-- 4. BERTH_ASSIGNMENTS TABLE
-- =====================================================
CREATE TABLE IF NOT EXISTS berth_assignments (
    id SERIAL PRIMARY KEY,
    ship_id INTEGER NOT NULL REFERENCES ships(id) ON DELETE CASCADE,
    berth_id INTEGER NOT NULL REFERENCES berths(id) ON DELETE CASCADE,
    eta TIMESTAMP NOT NULL,
    etd TIMESTAMP NOT NULL,
    actual_arrival_time TIMESTAMP,
    actual_departure_time TIMESTAMP,
    status VARCHAR(20) NOT NULL DEFAULT 'Scheduled' CHECK (status IN ('Scheduled', 'Arrived', 'Departed', 'Delayed', 'Cancelled')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_eta_before_etd CHECK (eta < etd)
);

-- Tambahkan kolom jika belum ada
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='ship_id') THEN
        ALTER TABLE berth_assignments ADD COLUMN ship_id INTEGER NOT NULL REFERENCES ships(id) ON DELETE CASCADE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='berth_id') THEN
        ALTER TABLE berth_assignments ADD COLUMN berth_id INTEGER NOT NULL REFERENCES berths(id) ON DELETE CASCADE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='eta') THEN
        ALTER TABLE berth_assignments ADD COLUMN eta TIMESTAMP NOT NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='etd') THEN
        ALTER TABLE berth_assignments ADD COLUMN etd TIMESTAMP NOT NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='actual_arrival_time') THEN
        ALTER TABLE berth_assignments ADD COLUMN actual_arrival_time TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='actual_departure_time') THEN
        ALTER TABLE berth_assignments ADD COLUMN actual_departure_time TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='status') THEN
        ALTER TABLE berth_assignments ADD COLUMN status VARCHAR(20) NOT NULL DEFAULT 'Scheduled' CHECK (status IN ('Scheduled', 'Arrived', 'Departed', 'Delayed', 'Cancelled'));
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='created_at') THEN
        ALTER TABLE berth_assignments ADD COLUMN created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='berth_assignments' AND column_name='updated_at') THEN
        ALTER TABLE berth_assignments ADD COLUMN updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;
END $$;

-- Indexes
CREATE INDEX IF NOT EXISTS idx_berth_assignments_ship ON berth_assignments(ship_id);
CREATE INDEX IF NOT EXISTS idx_berth_assignments_berth ON berth_assignments(berth_id);
CREATE INDEX IF NOT EXISTS idx_berth_assignments_dates ON berth_assignments(eta, etd);
CREATE INDEX IF NOT EXISTS idx_berth_assignments_status ON berth_assignments(status);

-- =====================================================
-- 5. INVOICES TABLE
-- =====================================================
CREATE TABLE IF NOT EXISTS invoices (
    id SERIAL PRIMARY KEY,
    berth_assignment_id INTEGER NOT NULL REFERENCES berth_assignments(id) ON DELETE CASCADE,
    total_amount DECIMAL(12,2) NOT NULL CHECK (total_amount >= 0),
    issued_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    due_date TIMESTAMP NOT NULL,
    is_paid BOOLEAN DEFAULT FALSE,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tambahkan kolom jika belum ada
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='invoices' AND column_name='berth_assignment_id') THEN
        ALTER TABLE invoices ADD COLUMN berth_assignment_id INTEGER NOT NULL REFERENCES berth_assignments(id) ON DELETE CASCADE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='invoices' AND column_name='total_amount') THEN
        ALTER TABLE invoices ADD COLUMN total_amount DECIMAL(12,2) NOT NULL CHECK (total_amount >= 0);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='invoices' AND column_name='issued_date') THEN
        ALTER TABLE invoices ADD COLUMN issued_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='invoices' AND column_name='due_date') THEN
        ALTER TABLE invoices ADD COLUMN due_date TIMESTAMP NOT NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='invoices' AND column_name='is_paid') THEN
        ALTER TABLE invoices ADD COLUMN is_paid BOOLEAN DEFAULT FALSE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='invoices' AND column_name='notes') THEN
        ALTER TABLE invoices ADD COLUMN notes TEXT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='invoices' AND column_name='created_at') THEN
        ALTER TABLE invoices ADD COLUMN created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='invoices' AND column_name='updated_at') THEN
        ALTER TABLE invoices ADD COLUMN updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;
END $$;

-- Indexes
CREATE INDEX IF NOT EXISTS idx_invoices_assignment ON invoices(berth_assignment_id);
CREATE INDEX IF NOT EXISTS idx_invoices_paid ON invoices(is_paid);
CREATE INDEX IF NOT EXISTS idx_invoices_due_date ON invoices(due_date);

-- =====================================================
-- 6. SHIP_TYPE_MULTIPLIERS TABLE
-- =====================================================
CREATE TABLE IF NOT EXISTS ship_type_multipliers (
    id SERIAL PRIMARY KEY,
    ship_type VARCHAR(50) NOT NULL UNIQUE CHECK (ship_type IN ('Container', 'Tanker', 'Bulk Carrier', 'General Cargo', 'Passenger')),
    multiplier DECIMAL(5,2) NOT NULL CHECK (multiplier > 0),
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tambahkan kolom jika belum ada
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ship_type_multipliers' AND column_name='ship_type') THEN
        ALTER TABLE ship_type_multipliers ADD COLUMN ship_type VARCHAR(50) NOT NULL UNIQUE CHECK (ship_type IN ('Container', 'Tanker', 'Bulk Carrier', 'General Cargo', 'Passenger'));
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ship_type_multipliers' AND column_name='multiplier') THEN
        ALTER TABLE ship_type_multipliers ADD COLUMN multiplier DECIMAL(5,2) NOT NULL CHECK (multiplier > 0);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ship_type_multipliers' AND column_name='description') THEN
        ALTER TABLE ship_type_multipliers ADD COLUMN description TEXT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ship_type_multipliers' AND column_name='created_at') THEN
        ALTER TABLE ship_type_multipliers ADD COLUMN created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='ship_type_multipliers' AND column_name='updated_at') THEN
        ALTER TABLE ship_type_multipliers ADD COLUMN updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_ship_type_mult_type ON ship_type_multipliers(ship_type);

-- =====================================================
-- 7. SIZE_MULTIPLIERS TABLE
-- =====================================================
CREATE TABLE IF NOT EXISTS size_multipliers (
    id SERIAL PRIMARY KEY,
    min_length DECIMAL(6,2) NOT NULL CHECK (min_length >= 0),
    max_length DECIMAL(6,2) NOT NULL CHECK (max_length > min_length),
    multiplier DECIMAL(5,2) NOT NULL CHECK (multiplier > 0),
    category VARCHAR(50) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tambahkan kolom jika belum ada
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='size_multipliers' AND column_name='min_length') THEN
        ALTER TABLE size_multipliers ADD COLUMN min_length DECIMAL(6,2) NOT NULL CHECK (min_length >= 0);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='size_multipliers' AND column_name='max_length') THEN
        ALTER TABLE size_multipliers ADD COLUMN max_length DECIMAL(6,2) NOT NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='size_multipliers' AND column_name='multiplier') THEN
        ALTER TABLE size_multipliers ADD COLUMN multiplier DECIMAL(5,2) NOT NULL CHECK (multiplier > 0);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='size_multipliers' AND column_name='category') THEN
        ALTER TABLE size_multipliers ADD COLUMN category VARCHAR(50) NOT NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='size_multipliers' AND column_name='created_at') THEN
        ALTER TABLE size_multipliers ADD COLUMN created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='size_multipliers' AND column_name='updated_at') THEN
        ALTER TABLE size_multipliers ADD COLUMN updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_size_mult_range ON size_multipliers(min_length, max_length);

-- =====================================================
-- INSERT SAMPLE DATA (jika tabel kosong)
-- =====================================================

-- Users sample data
INSERT INTO users (username, full_name, role, password_hash)
SELECT 'admin', 'Harbor Master Admin', 'HarborMaster', 'admin123'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'admin');

INSERT INTO users (username, full_name, role, password_hash)
SELECT 'operator1', 'John Operator', 'Operator', 'operator123'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'operator1');

-- Ship type multipliers sample data
INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Container', 1.5, 'Container ships require crane and yard services'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Container');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Tanker', 2.0, 'Tankers require hazmat handling and safety measures'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Tanker');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Bulk Carrier', 1.3, 'Bulk carriers need specialized loading equipment'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Bulk Carrier');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'General Cargo', 1.0, 'Standard cargo handling'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'General Cargo');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Passenger', 1.8, 'Passenger ships require terminal and customs services'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Passenger');

-- Size multipliers sample data
INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 0, 100, 1.0, 'Small Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Small Vessel');

INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 100, 200, 1.3, 'Medium Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Medium Vessel');

INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 200, 300, 1.6, 'Large Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Large Vessel');

INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 300, 500, 2.0, 'Very Large Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Very Large Vessel');

-- =====================================================
-- TRIGGERS UNTUK AUTO UPDATE TIMESTAMP
-- =====================================================

CREATE OR REPLACE FUNCTION fn_update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Drop existing triggers jika ada
DROP TRIGGER IF EXISTS trg_users_update ON users;
DROP TRIGGER IF EXISTS trg_ships_update ON ships;
DROP TRIGGER IF EXISTS trg_berths_update ON berths;
DROP TRIGGER IF EXISTS trg_berth_assignments_update ON berth_assignments;
DROP TRIGGER IF EXISTS trg_invoices_update ON invoices;
DROP TRIGGER IF EXISTS trg_ship_type_mult_update ON ship_type_multipliers;
DROP TRIGGER IF EXISTS trg_size_mult_update ON size_multipliers;

-- Buat triggers baru
CREATE TRIGGER trg_users_update BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

CREATE TRIGGER trg_ships_update BEFORE UPDATE ON ships
    FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

CREATE TRIGGER trg_berths_update BEFORE UPDATE ON berths
    FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

CREATE TRIGGER trg_berth_assignments_update BEFORE UPDATE ON berth_assignments
    FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

CREATE TRIGGER trg_invoices_update BEFORE UPDATE ON invoices
    FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

CREATE TRIGGER trg_ship_type_mult_update BEFORE UPDATE ON ship_type_multipliers
    FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

CREATE TRIGGER trg_size_mult_update BEFORE UPDATE ON size_multipliers
    FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

-- =====================================================
-- SELESAI
-- =====================================================
