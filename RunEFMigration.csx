// RunEFMigration.csx - kjør med: dotnet script RunEFMigration.csx
#r "Microsoft.Data.SqlClient"
using Microsoft.Data.SqlClient;

var connString = "Server=127.0.0.1,14333;Database=LagerProDb;User Id=sa;Password=LagerPro123!;Encrypt=False;TrustServerCertificate=True";

using var conn = new SqlConnection(connString);
conn.Open();

var cmd = new SqlCommand("INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20260401131504_RBAC_Initial', '8.0.0')", conn);
try {
    cmd.ExecuteNonQuery();
    Console.WriteLine("Ferdig! Migration markert som kjørt.");
} catch (SqlException ex) when (ex.Number == 2627) {
    Console.WriteLine("Allerede markert.");
}
