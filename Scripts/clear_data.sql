-- Clear all transactional and user data
-- Order is important due to Foreign Key constraints (DeleteBehavior.Restrict)
SET QUOTED_IDENTIFIER ON;

PRINT 'Clearing data...'

-- 1. Review related
DELETE FROM [ReviewPhotos];
DELETE FROM [Reviews];

-- 2. Communication & Quotes
DELETE FROM [Messages];
DELETE FROM [Quotes];

-- 3. Bookings (Central transaction table)
DELETE FROM [Bookings];

-- 4. Provider related details
DELETE FROM [ProviderPhotos];
DELETE FROM [ProviderAvailabilities];
DELETE FROM [ProviderServices];

-- 5. Core Entities
DELETE FROM [Providers];

-- 6. User Identity Tables
DELETE FROM [UserRoles];
DELETE FROM [UserClaims];
DELETE FROM [UserLogins];
DELETE FROM [UserTokens];
-- DELETE FROM [RoleClaims]; -- Usually system roles, safe to keep? Or delete if clean slate. Let's keep Roles.

-- 7. Users
DELETE FROM [Users];

-- Optional: Reset IDs (only if you want to start from ID 1 again, usually not necessary but nice)
-- DBCC CHECKIDENT ('[Users]', RESEED, 0); 
-- DBCC CHECKIDENT ('[Providers]', RESEED, 0);

PRINT 'All user and transaction data cleared.'
