using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LagerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPassordTilBruker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Passord",
                table: "Brukere",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "password");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Passord",
                table: "Brukere");
        }
    }
}
