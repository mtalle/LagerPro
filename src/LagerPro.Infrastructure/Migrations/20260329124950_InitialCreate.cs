using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LagerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtikkelNr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Beskrivelse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Enhet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Strekkode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Kategori = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Innpris = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Utpris = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinBeholdning = table.Column<int>(type: "int", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistEndret = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artikler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kunder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Navn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kontaktperson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Epost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Postnr = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Poststed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrgNr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistEndret = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kunder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leverandorer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Navn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kontaktperson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Epost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Postnr = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Poststed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrgNr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistEndret = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leverandorer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LagerBeholdninger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtikkelId = table.Column<int>(type: "int", nullable: false),
                    LotNr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mengde = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Enhet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Lokasjon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BestForDato = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SistOppdatert = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LagerBeholdninger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LagerBeholdninger_Artikler_ArtikkelId",
                        column: x => x.ArtikkelId,
                        principalTable: "Artikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LagerTransaksjoner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtikkelId = table.Column<int>(type: "int", nullable: false),
                    LotNr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Mengde = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    BeholdningEtter = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Kilde = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KildeId = table.Column<int>(type: "int", nullable: true),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UtfortAv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tidspunkt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LagerTransaksjoner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LagerTransaksjoner_Artikler_ArtikkelId",
                        column: x => x.ArtikkelId,
                        principalTable: "Artikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resepter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Navn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FerdigvareId = table.Column<int>(type: "int", nullable: false),
                    Beskrivelse = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AntallPortjoner = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Instruksjoner = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Versjon = table.Column<int>(type: "int", nullable: false),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistEndret = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resepter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resepter_Artikler_FerdigvareId",
                        column: x => x.FerdigvareId,
                        principalTable: "Artikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leveringer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KundeId = table.Column<int>(type: "int", nullable: false),
                    LeveringsDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Referanse = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FraktBrev = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LevertAv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leveringer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leveringer_Kunder_KundeId",
                        column: x => x.KundeId,
                        principalTable: "Kunder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mottak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeverandorId = table.Column<int>(type: "int", nullable: false),
                    MottaksDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Referanse = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MottattAv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mottak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mottak_Leverandorer_LeverandorId",
                        column: x => x.LeverandorId,
                        principalTable: "Leverandorer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProduksjonsOrdre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReseptId = table.Column<int>(type: "int", nullable: false),
                    OrdreNr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlanlagtDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FerdigmeldtDato = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AntallProdusert = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    FerdigvareLotNr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UtfortAv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProduksjonsOrdre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProduksjonsOrdre_Resepter_ReseptId",
                        column: x => x.ReseptId,
                        principalTable: "Resepter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReseptLinjer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReseptId = table.Column<int>(type: "int", nullable: false),
                    RavareId = table.Column<int>(type: "int", nullable: false),
                    Mengde = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Enhet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Rekkefolge = table.Column<int>(type: "int", nullable: false),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReseptLinjer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReseptLinjer_Artikler_RavareId",
                        column: x => x.RavareId,
                        principalTable: "Artikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReseptLinjer_Resepter_ReseptId",
                        column: x => x.ReseptId,
                        principalTable: "Resepter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeveringLinjer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeveringId = table.Column<int>(type: "int", nullable: false),
                    ArtikkelId = table.Column<int>(type: "int", nullable: false),
                    LotNr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mengde = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Enhet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeveringLinjer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeveringLinjer_Artikler_ArtikkelId",
                        column: x => x.ArtikkelId,
                        principalTable: "Artikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeveringLinjer_Leveringer_LeveringId",
                        column: x => x.LeveringId,
                        principalTable: "Leveringer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MottakLinjer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MottakId = table.Column<int>(type: "int", nullable: false),
                    ArtikkelId = table.Column<int>(type: "int", nullable: false),
                    LotNr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mengde = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Enhet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BestForDato = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Temperatur = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Strekkode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Avvik = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Godkjent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MottakLinjer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MottakLinjer_Artikler_ArtikkelId",
                        column: x => x.ArtikkelId,
                        principalTable: "Artikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MottakLinjer_Mottak_MottakId",
                        column: x => x.MottakId,
                        principalTable: "Mottak",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdOrdreForbruk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdOrdreId = table.Column<int>(type: "int", nullable: false),
                    ArtikkelId = table.Column<int>(type: "int", nullable: false),
                    LotNr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MengdeBrukt = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Enhet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Overstyrt = table.Column<bool>(type: "bit", nullable: false),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdOrdreForbruk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdOrdreForbruk_Artikler_ArtikkelId",
                        column: x => x.ArtikkelId,
                        principalTable: "Artikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProdOrdreForbruk_ProduksjonsOrdre_ProdOrdreId",
                        column: x => x.ProdOrdreId,
                        principalTable: "ProduksjonsOrdre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProduksjonsOrdreVersjoner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProduksjonsOrdreId = table.Column<int>(type: "int", nullable: false),
                    VersjonsNummer = table.Column<int>(type: "int", nullable: false),
                    AntallProdusert = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FerdigvareLotNr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Kommentar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UtfortAv = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FerdigmeldtDato = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpprettetDato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistEndret = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProduksjonsOrdreVersjoner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProduksjonsOrdreVersjoner_ProduksjonsOrdre_ProduksjonsOrdreId",
                        column: x => x.ProduksjonsOrdreId,
                        principalTable: "ProduksjonsOrdre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artikler_ArtikkelNr",
                table: "Artikler",
                column: "ArtikkelNr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LagerBeholdninger_ArtikkelId_LotNr",
                table: "LagerBeholdninger",
                columns: new[] { "ArtikkelId", "LotNr" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LagerTransaksjoner_ArtikkelId",
                table: "LagerTransaksjoner",
                column: "ArtikkelId");

            migrationBuilder.CreateIndex(
                name: "IX_Leveringer_KundeId",
                table: "Leveringer",
                column: "KundeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeveringLinjer_ArtikkelId",
                table: "LeveringLinjer",
                column: "ArtikkelId");

            migrationBuilder.CreateIndex(
                name: "IX_LeveringLinjer_LeveringId",
                table: "LeveringLinjer",
                column: "LeveringId");

            migrationBuilder.CreateIndex(
                name: "IX_Mottak_LeverandorId",
                table: "Mottak",
                column: "LeverandorId");

            migrationBuilder.CreateIndex(
                name: "IX_MottakLinjer_ArtikkelId",
                table: "MottakLinjer",
                column: "ArtikkelId");

            migrationBuilder.CreateIndex(
                name: "IX_MottakLinjer_MottakId",
                table: "MottakLinjer",
                column: "MottakId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdOrdreForbruk_ArtikkelId",
                table: "ProdOrdreForbruk",
                column: "ArtikkelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdOrdreForbruk_ProdOrdreId",
                table: "ProdOrdreForbruk",
                column: "ProdOrdreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProduksjonsOrdre_OrdreNr",
                table: "ProduksjonsOrdre",
                column: "OrdreNr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProduksjonsOrdre_ReseptId",
                table: "ProduksjonsOrdre",
                column: "ReseptId");

            migrationBuilder.CreateIndex(
                name: "IX_ProduksjonsOrdreVersjoner_ProduksjonsOrdreId",
                table: "ProduksjonsOrdreVersjoner",
                column: "ProduksjonsOrdreId");

            migrationBuilder.CreateIndex(
                name: "IX_Resepter_FerdigvareId",
                table: "Resepter",
                column: "FerdigvareId");

            migrationBuilder.CreateIndex(
                name: "IX_ReseptLinjer_RavareId",
                table: "ReseptLinjer",
                column: "RavareId");

            migrationBuilder.CreateIndex(
                name: "IX_ReseptLinjer_ReseptId",
                table: "ReseptLinjer",
                column: "ReseptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LagerBeholdninger");

            migrationBuilder.DropTable(
                name: "LagerTransaksjoner");

            migrationBuilder.DropTable(
                name: "LeveringLinjer");

            migrationBuilder.DropTable(
                name: "MottakLinjer");

            migrationBuilder.DropTable(
                name: "ProdOrdreForbruk");

            migrationBuilder.DropTable(
                name: "ProduksjonsOrdreVersjoner");

            migrationBuilder.DropTable(
                name: "ReseptLinjer");

            migrationBuilder.DropTable(
                name: "Leveringer");

            migrationBuilder.DropTable(
                name: "Mottak");

            migrationBuilder.DropTable(
                name: "ProduksjonsOrdre");

            migrationBuilder.DropTable(
                name: "Kunder");

            migrationBuilder.DropTable(
                name: "Leverandorer");

            migrationBuilder.DropTable(
                name: "Resepter");

            migrationBuilder.DropTable(
                name: "Artikler");
        }
    }
}
