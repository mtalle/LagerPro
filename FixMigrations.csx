// FixMigrations.csx - kjør med: dotnet script FixMigrations.csx
// Markerer RBAC-migreringa som kjøyrt utan å kjøyra SQL

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddDbContext<LagerPro.Infrastructure.Persistence.LagerProDbContext>();

var provider = services.BuildServiceProvider();
var context = provider.GetRequiredService<LagerPro.Infrastructure.Persistence.LagerProDbContext();

try
{
    var migrationExists = await context.Database.GetService<IMigrationsHistory>()
        .ExistsAsync();

    if (!await context.Database.CanConnectAsync())
    {
        Console.WriteLine("Cannot connect to database");
        return;
    }

    var history = context.Database.GetService<IMigrationsHistory>();
    await context.Database.OpenConnectionAsync();

    var migrationId = "20260401131504_RBAC_Initial";
    var productVersion = "8.0.0";

    try
    {
        await (( RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>())
            .CreateMigrationRecord(migrationId, productVersion);
        Console.WriteLine($"Migration {migrationId} marked as applied");
    }
    catch (Exception ex) when (ex.Message.Contains("duplicate"))
    {
        Console.WriteLine($"Migration {migrationId} already marked as applied");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
