-- ============================================================================
-- Migration: Remove Username, Use Email for Login
-- Date: 2025-11-29
-- Description: Migrates user authentication from username to email-based login
-- ============================================================================

-- IMPORTANT: Backup your database before running this migration!

-- ============================================================================
-- STEP 1: Data Migration - Preserve existing data
-- ============================================================================

-- 1.1: Backup username to full_name if full_name is empty
UPDATE users
SET full_name = username
WHERE full_name IS NULL OR full_name = '';

-- 1.2: Generate email for users who don't have one yet
-- IMPORTANT: Review and update these emails with real email addresses!
UPDATE users
SET email = username || '@harbor.local'
WHERE email IS NULL OR email = '';

-- 1.3: Verify all users have email and full_name
-- Run this query to check - should return 0 rows:
SELECT id, username, email, full_name
FROM users
WHERE email IS NULL OR email = '' OR full_name IS NULL OR full_name = '';

-- ============================================================================
-- STEP 2: Add Database Constraints
-- ============================================================================

-- 2.1: Make email NOT NULL and UNIQUE (required for login)
ALTER TABLE users
  ALTER COLUMN email SET NOT NULL;

ALTER TABLE users
  ADD CONSTRAINT users_email_unique UNIQUE (email);

-- 2.2: Make full_name NOT NULL (required for display)
ALTER TABLE users
  ALTER COLUMN full_name SET NOT NULL;

-- 2.3: Make username nullable (no longer used for login)
ALTER TABLE users
  ALTER COLUMN username DROP NOT NULL;

-- ============================================================================
-- STEP 3: Cleanup (OPTIONAL - Run after verifying everything works)
-- ============================================================================

-- WARNING: Only run this after thorough testing!
-- This will permanently delete the username column

-- To drop username column (uncomment when ready):
-- ALTER TABLE users DROP COLUMN username;

-- ============================================================================
-- VERIFICATION QUERIES
-- ============================================================================

-- Check all users have valid email and full_name:
SELECT id, email, full_name, role
FROM users
ORDER BY id;

-- Check for duplicate emails (should return 0 rows):
SELECT email, COUNT(*)
FROM users
GROUP BY email
HAVING COUNT(*) > 1;

-- ============================================================================
-- ROLLBACK (if needed)
-- ============================================================================

-- If you need to rollback (before dropping username column):
-- ALTER TABLE users DROP CONSTRAINT IF EXISTS users_email_unique;
-- ALTER TABLE users ALTER COLUMN email DROP NOT NULL;
-- ALTER TABLE users ALTER COLUMN full_name DROP NOT NULL;
