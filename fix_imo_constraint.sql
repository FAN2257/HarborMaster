-- =====================================================
-- Fix IMO Number Unique Constraint
-- Problem: IMO should be unique only for non-null/non-empty values
-- =====================================================

-- Drop existing unique constraint if exists
DO $$
BEGIN
    -- Drop unique constraint on imo_number column
    IF EXISTS (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'ships_imo_number_key'
    ) THEN
        ALTER TABLE ships DROP CONSTRAINT ships_imo_number_key;
    END IF;
END $$;

-- Drop existing unique index if exists
DROP INDEX IF EXISTS idx_ships_imo;

-- Create partial unique index (only for non-null and non-empty IMO numbers)
CREATE UNIQUE INDEX idx_ships_imo_unique
ON ships(imo_number)
WHERE imo_number IS NOT NULL AND imo_number != '';

-- Also create regular index for search performance
CREATE INDEX idx_ships_imo_search ON ships(imo_number);

COMMENT ON INDEX idx_ships_imo_unique IS 'Unique constraint for IMO numbers, allowing multiple NULL/empty values';
