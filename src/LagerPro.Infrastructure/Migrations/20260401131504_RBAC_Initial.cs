using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LagerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RBAC_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brukere",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Navn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Brukernavn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Epost = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ErAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistEndret = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brukere", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ressurser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Navn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Beskrivelse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistEndret = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ressurser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrukerRessursTilganger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrukerId = table.Column<int>(type: "int", nullable: false),
                    RessursId = table.Column<int>(type: "int", nullable: false),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistEndret = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrukerRessursTilganger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrukerRessursTilganger_Brukere_BrukerId",
                        column: x => x.BrukerId,
                        principalTable: "Brukere",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrukerRessursTilganger_Ressurser_RessursId",
                        column: x => x.RessursId,
                        principalTable: "Ressurser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leverandorer_OrgNr",
                table: "Leverandorer",
                column: "OrgNr",
                unique: true,
                filter: "[OrgNr] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Kunder_OrgNr",
                table: "Kunder",
                column: "OrgNr",
                unique: true,
                filter: "[OrgNr] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BrukerRessursTilganger_BrukerId_RessursId",
                table: "BrukerRessursTilganger",
                columns: new[] { "BrukerId", "RessursId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrukerRessursTilganger_RessursId",
                table: "BrukerRessursTilganger",
                column: "RessursId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrukerRessursTilganger");

            migrationBuilder.DropTable(
                name: "Brukere");

            migrationBuilder.DropTable(
                name: "Ressurser");

            migrationBuilder.DropIndex(
                name: "IX_Leverandorer_OrgNr",
                table: "Leverandorer");

            migrationBuilder.DropIndex(
                name: "IX_Kunder_OrgNr",
                table: "Kunder");
        }
    }
}
