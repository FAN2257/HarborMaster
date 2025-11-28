-- Migration: Add user profile fields
-- Date: 2025-11-28
-- Description: Adds email, phone, and company_name columns to users table

-- Add email column
ALTER TABLE users
ADD COLUMN IF NOT EXISTS email VARCHAR(255);

-- Add phone column
ALTER TABLE users
ADD COLUMN IF NOT EXISTS phone VARCHAR(50);

-- Add company_name column (especially for ShipOwner role)
ALTER TABLE users
ADD COLUMN IF NOT EXISTS company_name VARCHAR(255);

-- Add indexes for better query performance
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);

-- Add comment for documentation
COMMENT ON COLUMN users.email IS 'User email address for notifications and contact';
COMMENT ON COLUMN users.phone IS 'User phone number for emergency contact';
COMMENT ON COLUMN users.company_name IS 'Company/organization name (primarily for ShipOwner role)';
