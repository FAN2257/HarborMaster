-- =====================================================
-- SEED DATA untuk HarborMaster Database
-- Jalankan setelah migration untuk populate data awal
-- =====================================================

-- =====================================================
-- 1. BERTHS (Dermaga) - Data Awal
-- =====================================================
-- Dermaga untuk kapal kecil-menengah
INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth A1', 'North Dock A', 150.00, 8.00, 'Available', 800.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth A1');

INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth A2', 'North Dock A', 180.00, 10.00, 'Available', 1000.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth A2');

INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth A3', 'North Dock A', 200.00, 12.00, 'Available', 1200.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth A3');

-- Dermaga untuk kapal besar
INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth B1', 'South Dock B', 250.00, 15.00, 'Available', 1500.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth B1');

INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth B2', 'South Dock B', 300.00, 18.00, 'Available', 2000.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth B2');

INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth B3', 'South Dock B', 350.00, 20.00, 'Available', 2500.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth B3');

-- Dermaga untuk kapal sangat besar (container, tanker)
INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth C1', 'Deep Water Terminal', 400.00, 22.00, 'Available', 3000.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth C1');

INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth C2', 'Deep Water Terminal', 450.00, 25.00, 'Available', 3500.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth C2');

-- Dermaga sedang maintenance (contoh)
INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth D1', 'East Dock', 220.00, 13.00, 'Maintenance', 1300.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth D1');

-- Dermaga untuk passenger ships
INSERT INTO berths (berth_name, location, max_length, max_draft, status, base_rate_per_day)
SELECT 'Berth P1', 'Passenger Terminal', 280.00, 14.00, 'Available', 1800.00
WHERE NOT EXISTS (SELECT 1 FROM berths WHERE berth_name = 'Berth P1');


-- =====================================================
-- 2. SHIP TYPE MULTIPLIERS
-- =====================================================
INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Container', 1.2, 'Container ships - standard rate'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Container');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Tanker', 1.5, 'Tanker ships - hazmat handling premium'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Tanker');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Bulk Carrier', 1.1, 'Bulk carrier ships - minor premium'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Bulk Carrier');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'General Cargo', 1.0, 'General cargo ships - base rate'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'General Cargo');

INSERT INTO ship_type_multipliers (ship_type, multiplier, description)
SELECT 'Passenger', 1.3, 'Passenger ships - terminal services premium'
WHERE NOT EXISTS (SELECT 1 FROM ship_type_multipliers WHERE ship_type = 'Passenger');


-- =====================================================
-- 3. SIZE MULTIPLIERS
-- =====================================================
INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 0, 100, 0.8, 'Small Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Small Vessel');

INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 100, 200, 1.0, 'Medium Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Medium Vessel');

INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 200, 300, 1.3, 'Large Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Large Vessel');

INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 300, 500, 1.6, 'Very Large Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Very Large Vessel');

INSERT INTO size_multipliers (min_length, max_length, multiplier, category)
SELECT 500, 9999, 2.0, 'Super Large Vessel'
WHERE NOT EXISTS (SELECT 1 FROM size_multipliers WHERE category = 'Super Large Vessel');


-- =====================================================
-- 4. DEMO USERS (Optional - untuk testing)
-- =====================================================
-- Password: "password123" (plain text untuk development)
INSERT INTO users (username, full_name, password_hash, role)
SELECT 'shipowner1', 'PT Pelayaran Indonesia', 'password123', 'ShipOwner'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'shipowner1');

INSERT INTO users (username, full_name, password_hash, role)
SELECT 'shipowner2', 'Maersk Shipping', 'password123', 'ShipOwner'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'shipowner2');

INSERT INTO users (username, full_name, password_hash, role)
SELECT 'operator1', 'Operator Pelabuhan', 'password123', 'Operator'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'operator1');

INSERT INTO users (username, full_name, password_hash, role)
SELECT 'harbormaster', 'Harbor Master', 'password123', 'HarborMaster'
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'harbormaster');


-- =====================================================
-- 5. DEMO SHIPS (Optional - untuk testing)
-- =====================================================
-- Ships untuk ship owner 1
INSERT INTO ships (name, imo_number, length_overall, draft, ship_type, owner_id)
SELECT
    'MV Jakarta Express', 'IMO1234567', 180.00, 10.50, 'Container',
    (SELECT id FROM users WHERE username = 'shipowner1' LIMIT 1)
WHERE EXISTS (SELECT 1 FROM users WHERE username = 'shipowner1')
ON CONFLICT DO NOTHING;

INSERT INTO ships (name, imo_number, length_overall, draft, ship_type, owner_id)
SELECT
    'MV Pertamina Oil', 'IMO2345678', 220.00, 12.00, 'Tanker',
    (SELECT id FROM users WHERE username = 'shipowner1' LIMIT 1)
WHERE EXISTS (SELECT 1 FROM users WHERE username = 'shipowner1')
ON CONFLICT DO NOTHING;

-- Ships untuk ship owner 2
INSERT INTO ships (name, imo_number, length_overall, draft, ship_type, owner_id)
SELECT
    'MS Dream Cruise', 'IMO3456789', 250.00, 13.50, 'Passenger',
    (SELECT id FROM users WHERE username = 'shipowner2' LIMIT 1)
WHERE EXISTS (SELECT 1 FROM users WHERE username = 'shipowner2')
ON CONFLICT DO NOTHING;


-- =====================================================
-- 6. VERIFICATION
-- =====================================================
-- Tampilkan summary data yang sudah di-insert
SELECT 'Berths created:' as info, COUNT(*) as count FROM berths
UNION ALL
SELECT 'Ship types configured:', COUNT(*) FROM ship_type_multipliers
UNION ALL
SELECT 'Size multipliers configured:', COUNT(*) FROM size_multipliers
UNION ALL
SELECT 'Demo users created:', COUNT(*) FROM users
UNION ALL
SELECT 'Demo ships created:', COUNT(*) FROM ships;

-- Tampilkan available berths
SELECT
    berth_name,
    location,
    max_length || 'm' as max_length,
    max_draft || 'm' as max_draft,
    status,
    '$' || base_rate_per_day || '/day' as rate
FROM berths
ORDER BY max_length;
