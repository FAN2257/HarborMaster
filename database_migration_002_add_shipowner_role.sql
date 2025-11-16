-- =====================================================
-- MIGRATION 002: Add ShipOwner role support
-- Date: 2025-01-16
-- Purpose: Update users table to support ShipOwner role
-- =====================================================

-- =====================================================
-- STEP 1: Check if user_role enum exists and add ShipOwner value
-- =====================================================
DO $$
BEGIN
    -- Check if the enum type exists
    IF EXISTS (SELECT 1 FROM pg_type WHERE typname = 'user_role') THEN
        -- Add 'ShipOwner' to the enum if it doesn't already exist
        IF NOT EXISTS (
            SELECT 1 FROM pg_enum
            WHERE enumtypid = 'user_role'::regtype
            AND enumlabel = 'ShipOwner'
        ) THEN
            ALTER TYPE user_role ADD VALUE 'ShipOwner';
        END IF;
    END IF;
END $$;

-- =====================================================
-- STEP 2: If role is VARCHAR with constraint, update the constraint
-- =====================================================
DO $$
BEGIN
    -- Drop the old constraint if it exists (for VARCHAR type)
    IF EXISTS (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'users_role_check'
    ) THEN
        ALTER TABLE users DROP CONSTRAINT users_role_check;

        -- Add new constraint with ShipOwner included
        ALTER TABLE users
        ADD CONSTRAINT users_role_check
        CHECK (role IN ('ShipOwner', 'Operator', 'HarborMaster'));
    END IF;
END $$;

-- =====================================================
-- Verification
-- =====================================================
SELECT
    constraint_name,
    check_clause
FROM information_schema.check_constraints
WHERE constraint_name = 'users_role_check';

-- Show existing users
SELECT username, full_name, role FROM users;
