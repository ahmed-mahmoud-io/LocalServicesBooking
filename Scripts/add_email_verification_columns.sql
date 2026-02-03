-- Add missing columns if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Users]') AND name = 'EmailVerificationCode')
BEGIN
    ALTER TABLE [Users] ADD [EmailVerificationCode] nvarchar(6) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Users]') AND name = 'EmailVerificationCodeAttempts')
BEGIN
    ALTER TABLE [Users] ADD [EmailVerificationCodeAttempts] int NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Users]') AND name = 'EmailVerificationCodeSentAt')
BEGIN
    ALTER TABLE [Users] ADD [EmailVerificationCodeSentAt] datetime2 NULL;
END

PRINT 'Columns added successfully!'
