-- ============================================
-- Migration: Fix Users Table Auto-Increment
-- Date: 2025-01-XX
-- Purpose: Ensure 'id' column auto-increments properly
-- ============================================

-- 1. Check if sequence exists, if not create it
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_sequences 
        WHERE schemaname = 'public' AND sequencename = 'users_id_seq'
    ) THEN
        -- Create sequence
        CREATE SEQUENCE public.users_id_seq;
        
        -- Set sequence to start from max existing id + 1
        PERFORM setval('public.users_id_seq', 
            COALESCE((SELECT MAX(id) FROM public.users), 0) + 1, 
            false
        );
        
        -- Alter column to use sequence
        ALTER TABLE public.users 
        ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq');
        
        -- Set sequence owner
        ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;
        
        RAISE NOTICE 'Sequence users_id_seq created successfully';
    ELSE
        RAISE NOTICE 'Sequence users_id_seq already exists';
    END IF;
END $$;

-- 2. Ensure primary key constraint exists
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'users_pkey' AND contype = 'p'
    ) THEN
        ALTER TABLE public.users ADD CONSTRAINT users_pkey PRIMARY KEY (id);
        RAISE NOTICE 'Primary key constraint added';
    ELSE
        RAISE NOTICE 'Primary key constraint already exists';
    END IF;
END $$;

-- 3. Create unique constraint on email (if not exists)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'users_email_key'
    ) THEN
        ALTER TABLE public.users ADD CONSTRAINT users_email_key UNIQUE (email);
        RAISE NOTICE 'Email unique constraint added';
    ELSE
        RAISE NOTICE 'Email unique constraint already exists';
    END IF;
END $$;

-- 4. Verify table structure
SELECT 
    column_name,
    data_type,
    column_default,
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'users'
ORDER BY ordinal_position;

-- Expected output:
-- id         | integer | nextval('users_id_seq'::regclass) | NO
-- username   | text    |                                   | YES
-- email      | text    |                                   | YES
-- full_name  | text    |                                   | YES
-- role       | text    |                                   | YES
-- password_hash | text |                                   | YES

COMMENT ON TABLE public.users IS 'User accounts with role-based permissions';
COMMENT ON COLUMN public.users.id IS 'Auto-increment primary key';
COMMENT ON COLUMN public.users.email IS 'User email address (unique)';
COMMENT ON COLUMN public.users.role IS 'User role: ShipOwner, Operator, or HarborMaster';
