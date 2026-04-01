-- FixMigrations.sql
-- Kjør dette mot databasen for å markera RBAC-migreringa som kjøyrt
-- Kjør med: docker exec -i lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost,14333 -U sa -P "LagerPro123!" -d LagerProDb -i FixMigrations.sql

USE LagerProDb;
GO

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260401131504_RBAC_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20260401131504_RBAC_Initial', '8.0.0');
    PRINT 'Migration marked as applied';
END
ELSE
BEGIN
    PRINT 'Migration already marked as applied';
END
