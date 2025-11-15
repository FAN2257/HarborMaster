-- =====================================================
-- HARBORMASTER DATABASE MIGRATION #001
-- Date: 2025-01-12
-- Purpose: Add ownership, berth status, and docking requests
-- =====================================================

-- =====================================================
-- STEP 1: Add owner_id to ships table
-- =====================================================
ALTER TABLE ships
ADD COLUMN IF NOT EXISTS owner_id INTEGER REFERENCES users(id);

-- Add comment
COMMENT ON COLUMN ships.owner_id IS 'User ID of the ship owner';

-- Create index for faster queries
CREATE INDEX IF NOT EXISTS idx_ships_owner_id ON ships(owner_id);


-- =====================================================
-- STEP 2: Add status column to berths table
-- =====================================================
ALTER TABLE berths
ADD COLUMN IF NOT EXISTS status VARCHAR(20) DEFAULT 'Available';

-- Add comment
COMMENT ON COLUMN berths.status IS 'Berth status: Available, Occupied, Maintenance, Damaged';

-- Add check constraint to ensure valid status values (with existence check)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'berths_status_check'
    ) THEN
        ALTER TABLE berths
        ADD CONSTRAINT berths_status_check
        CHECK (status IN ('Available', 'Occupied', 'Maintenance', 'Damaged'));
    END IF;
END $$;

-- Create index for filtering by status
CREATE INDEX IF NOT EXISTS idx_berths_status ON berths(status);


-- =====================================================
-- STEP 3: Create docking_requests table
-- =====================================================
CREATE TABLE IF NOT EXISTS docking_requests (
    id SERIAL PRIMARY KEY,
    ship_id INTEGER NOT NULL REFERENCES ships(id) ON DELETE CASCADE,
    owner_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    requested_eta TIMESTAMP NOT NULL,
    requested_etd TIMESTAMP NOT NULL,
    cargo_type VARCHAR(100),
    special_requirements TEXT,
    status VARCHAR(20) DEFAULT 'Pending' NOT NULL,
    created_at TIMESTAMP DEFAULT NOW() NOT NULL,
    processed_by INTEGER REFERENCES users(id),
    processed_at TIMESTAMP,
    rejection_reason TEXT,
    berth_assignment_id INTEGER REFERENCES berth_assignments(id)
);

-- Add constraints separately with existence checks
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'docking_requests_status_check'
    ) THEN
        ALTER TABLE docking_requests
        ADD CONSTRAINT docking_requests_status_check
        CHECK (status IN ('Pending', 'Approved', 'Rejected', 'Cancelled'));
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'docking_requests_eta_etd_check'
    ) THEN
        ALTER TABLE docking_requests
        ADD CONSTRAINT docking_requests_eta_etd_check
        CHECK (requested_eta < requested_etd);
    END IF;
END $$;

-- Add comments
COMMENT ON TABLE docking_requests IS 'Ship owner docking requests awaiting operator approval';
COMMENT ON COLUMN docking_requests.status IS 'Request status: Pending, Approved, Rejected, Cancelled';
COMMENT ON COLUMN docking_requests.processed_by IS 'Operator/HarborMaster who processed the request';
COMMENT ON COLUMN docking_requests.berth_assignment_id IS 'Linked berth assignment if approved';

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS idx_docking_requests_ship_id ON docking_requests(ship_id);
CREATE INDEX IF NOT EXISTS idx_docking_requests_owner_id ON docking_requests(owner_id);
CREATE INDEX IF NOT EXISTS idx_docking_requests_status ON docking_requests(status);
CREATE INDEX IF NOT EXISTS idx_docking_requests_created_at ON docking_requests(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_docking_requests_processed_by ON docking_requests(processed_by);


-- =====================================================
-- STEP 4: Update existing data (optional, for demo)
-- =====================================================

-- Set all existing ships to a default owner (user with id=1)
-- UNCOMMENT if you want to assign existing ships to first user
-- UPDATE ships
-- SET owner_id = 1
-- WHERE owner_id IS NULL AND EXISTS (SELECT 1 FROM users WHERE id = 1);

-- Set all existing berths to Available status
UPDATE berths
SET status = 'Available'
WHERE status IS NULL OR status = '';


-- =====================================================
-- STEP 5: Verification queries
-- =====================================================

-- Verify ships table structure
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'ships'
ORDER BY ordinal_position;

-- Verify berths table structure
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'berths'
ORDER BY ordinal_position;

-- Verify docking_requests table structure
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'docking_requests'
ORDER BY ordinal_position;

-- Show table counts
SELECT
    'ships' as table_name, COUNT(*) as row_count FROM ships
UNION ALL
SELECT 'berths', COUNT(*) FROM berths
UNION ALL
SELECT 'docking_requests', COUNT(*) FROM docking_requests
UNION ALL
SELECT 'users', COUNT(*) FROM users;

-- =====================================================
-- END OF MIGRATION
-- =====================================================
