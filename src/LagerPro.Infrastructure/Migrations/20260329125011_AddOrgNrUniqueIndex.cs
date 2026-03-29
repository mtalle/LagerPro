using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LagerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgNrUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Leverandorer_OrgNr",
                table: "Leverandorer",
                column: "OrgNr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kunder_OrgNr",
                table: "Kunder",
                column: "OrgNr",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leverandorer_OrgNr",
                table: "Leverandorer");

            migrationBuilder.DropIndex(
                name: "IX_Kunder_OrgNr",
                table: "Kunder");
        }
    }
}
