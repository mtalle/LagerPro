using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByArtikkel;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByBatch;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByKunde;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByLot;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;
using Moq;

namespace LagerPro.Application.Tests;

public class TraceabilityTests
{
    private readonly Mock<ILagerRepository> _lagerRepoMock;
    private readonly Mock<ILagerTransaksjonRepository> _transaksjonRepoMock;
    private readonly Mock<IArtikkelRepository> _artikkelRepoMock;
    private readonly Mock<IProduksjonsOrdreRepository> _ordreRepoMock;
    private readonly Mock<IKundeRepository> _kundeRepoMock;
    private readonly Mock<ILeveringRepository> _leveringRepoMock;

    public TraceabilityTests()
    {
        _lagerRepoMock = new Mock<ILagerRepository>();
        _transaksjonRepoMock = new Mock<ILagerTransaksjonRepository>();
        _artikkelRepoMock = new Mock<IArtikkelRepository>();
        _ordreRepoMock = new Mock<IProduksjonsOrdreRepository>();
        _kundeRepoMock = new Mock<IKundeRepository>();
        _leveringRepoMock = new Mock<ILeveringRepository>();
    }

    #region GetTraceabilityByLotHandler

    [Fact]
    public async Task GetTraceabilityByLotHandler_LotExists_ReturnsFullTrace()
    {
        var beholdning = new LagerBeholdning
        {
            ArtikkelId = 10,
            LotNr = "LOT-001",
            Mengde = 50,
            Enhet = "kg",
            BestForDato = DateTime.UtcNow.AddMonths(3),
            SistOppdatert = DateTime.UtcNow,
            Lokasjon = "Hylle A1",
            Artikkel = new Artikkel { ArtikkelNr = "V001", Navn = "Salt" }
        };

        var transaksjoner = new List<LagerTransaksjon>
        {
            CreateTransaksjon(1, 10, "LOT-001", TransaksjonsType.Mottak, 100),
            CreateTransaksjon(2, 10, "LOT-001", TransaksjonsType.ProduksjonUttak, 50),
        };

        _lagerRepoMock.Setup(r => r.GetByLotNrAsync("LOT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(beholdning);
        _transaksjonRepoMock.Setup(r => r.GetByArtikkelAndLotAsync(10, "LOT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaksjoner);

        var handler = new GetTraceabilityByLotHandler(_lagerRepoMock.Object, _transaksjonRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByLotQuery("LOT-001"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("LOT-001", result.LotNr);
        Assert.Equal(10, result.ArtikkelId);
        Assert.Equal("V001", result.ArtikkelNr);
        Assert.Equal("Salt", result.ArtikkelNavn);
        Assert.Equal(50, result.Mengde);
        Assert.Equal("kg", result.Enhet);
        Assert.Equal(2, result.Transaksjoner.Count);
    }

    [Fact]
    public async Task GetTraceabilityByLotHandler_LotNotFound_ReturnsNull()
    {
        _lagerRepoMock.Setup(r => r.GetByLotNrAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((LagerBeholdning?)null);

        var handler = new GetTraceabilityByLotHandler(_lagerRepoMock.Object, _transaksjonRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByLotQuery("NONEXISTENT"), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region GetTraceabilityByArtikkelHandler

    [Fact]
    public async Task GetTraceabilityByArtikkelHandler_ArtikkelExists_ReturnsAllLotsAndTransactions()
    {
        var artikkel = new Artikkel { ArtikkelNr = "V001", Navn = "Salt", Enhet = "kg" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(artikkel, 10);

        var beholdninger = new List<LagerBeholdning>
        {
            new() { ArtikkelId = 10, LotNr = "LOT-001", Mengde = 50, Enhet = "kg" },
            new() { ArtikkelId = 10, LotNr = "LOT-002", Mengde = 30, Enhet = "kg" }
        };

        var transaksjoner = new List<LagerTransaksjon>
        {
            CreateTransaksjon(1, 10, "LOT-001", TransaksjonsType.Mottak, 100),
            CreateTransaksjon(2, 10, "LOT-001", TransaksjonsType.ProduksjonUttak, 50),
            CreateTransaksjon(3, 10, "LOT-002", TransaksjonsType.Mottak, 30),
        };

        _artikkelRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(artikkel);
        _lagerRepoMock.Setup(r => r.GetByArtikkelAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(beholdninger);
        _transaksjonRepoMock.Setup(r => r.GetByArtikkelIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(transaksjoner);

        var handler = new GetTraceabilityByArtikkelHandler(_artikkelRepoMock.Object, _lagerRepoMock.Object, _transaksjonRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByArtikkelQuery(10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.ArtikkelId);
        Assert.Equal("V001", result.ArtikkelNr);
        Assert.Equal(2, result.Lotter.Count);
    }

    [Fact]
    public async Task GetTraceabilityByArtikkelHandler_ArtikkelNotFound_ReturnsNull()
    {
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artikkel?)null);

        var handler = new GetTraceabilityByArtikkelHandler(_artikkelRepoMock.Object, _lagerRepoMock.Object, _transaksjonRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByArtikkelQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region GetTraceabilityByBatchHandler

    [Fact]
    public async Task GetTraceabilityByBatchHandler_BatchExists_ReturnsBatchDetailsWithForbruk()
    {
        var ordre = CreateTestProduksjonsOrdre(5, "PO-001", ProdOrdreStatus.Ferdigmeldt);
        ordre.Forbruk.Add(new ProdOrdreForbruk
        {
            ArtikkelId = 10,
            LotNr = "LOT-001",
            MengdeBrukt = 5,
            Enhet = "kg"
        });

        var transaksjoner = new List<LagerTransaksjon>
        {
            CreateTransaksjon(1, 10, "LOT-001", TransaksjonsType.ProduksjonUttak, 5),
            CreateTransaksjon(2, 20, "FG-001", TransaksjonsType.ProduksjonInn, 10),
        };

        _ordreRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(ordre);
        _transaksjonRepoMock.Setup(r => r.GetByBatchNrAsync("5", It.IsAny<CancellationToken>())).ReturnsAsync(transaksjoner);

        var handler = new GetTraceabilityByBatchHandler(_ordreRepoMock.Object, _transaksjonRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByBatchQuery("5"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result.OrdreId);
        Assert.Equal("PO-001", result.OrdreNr);
        Assert.Single(result.Forbruk);
        Assert.Equal(2, result.Transaksjoner.Count);
    }

    [Fact]
    public async Task GetTraceabilityByBatchHandler_InvalidBatchNr_ReturnsNull()
    {
        var handler = new GetTraceabilityByBatchHandler(_ordreRepoMock.Object, _transaksjonRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByBatchQuery("invalid"), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTraceabilityByBatchHandler_BatchNotFound_ReturnsNull()
    {
        _ordreRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProduksjonsOrdre?)null);

        var handler = new GetTraceabilityByBatchHandler(_ordreRepoMock.Object, _transaksjonRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByBatchQuery("999"), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region GetTraceabilityByKundeHandler

    [Fact]
    public async Task GetTraceabilityByKundeHandler_KundeExists_ReturnsAllLeveringer()
    {
        var kunde = CreateTestKunde(1, "Brenneriet AS");
        var leveringer = new List<Levering>
        {
            CreateTestLevering(10, 1, LeveringStatus.Levert),
            CreateTestLevering(11, 1, LeveringStatus.Sendt)
        };

        _kundeRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(kunde);
        _leveringRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(leveringer);

        var handler = new GetTraceabilityByKundeHandler(_kundeRepoMock.Object, _leveringRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByKundeQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.KundeId);
        Assert.Equal("Brenneriet AS", result.KundeNavn);
        Assert.Equal(2, result.Leveringer.Count);
    }

    [Fact]
    public async Task GetTraceabilityByKundeHandler_KundeNotFound_ReturnsNull()
    {
        _kundeRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Kunde?)null);

        var handler = new GetTraceabilityByKundeHandler(_kundeRepoMock.Object, _leveringRepoMock.Object);

        var result = await handler.Handle(new GetTraceabilityByKundeQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    // --- Helpers ---
    private static LagerTransaksjon CreateTransaksjon(int id, int artikkelId, string lotNr, TransaksjonsType type, decimal mengde)
    {
        var t = new LagerTransaksjon
        {
            ArtikkelId = artikkelId,
            LotNr = lotNr,
            Type = type,
            Mengde = mengde,
            BeholdningEtter = mengde,
            Kilde = type.ToString(),
            KildeId = id,
            Tidspunkt = DateTime.UtcNow
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(t, id);
        return t;
    }

    private static ProduksjonsOrdre CreateTestProduksjonsOrdre(int id, string ordreNr, ProdOrdreStatus status)
    {
        var ordre = new ProduksjonsOrdre
        {
            ReseptId = 1,
            OrdreNr = ordreNr,
            PlanlagtDato = DateTime.UtcNow,
            Status = status,
            FerdigvareLotNr = "FG-001",
            Forbruk = new List<ProdOrdreForbruk>()
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ordre, id);
        return ordre;
    }

    private static Kunde CreateTestKunde(int id, string navn)
    {
        var kunde = new Kunde { Navn = navn, Aktiv = true };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(kunde, id);
        return kunde;
    }

    private static Levering CreateTestLevering(int id, int kundeId, LeveringStatus status)
    {
        var levering = new Levering
        {
            KundeId = kundeId,
            LeveringsDato = DateTime.UtcNow,
            Status = status,
            Linjer = new List<LeveringLinje>()
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(levering, id);
        return levering;
    }
}
