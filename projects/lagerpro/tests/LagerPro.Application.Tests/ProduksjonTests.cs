using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.FerdigmeldProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.UpdateProduksjonsOrdreStatus;
using LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Queries.GetProduksjonsOrdreById;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;
using Moq;

namespace LagerPro.Application.Tests;

public class ProduksjonTests
{
    private readonly Mock<IProduksjonsOrdreRepository> _ordreRepoMock;
    private readonly Mock<IReseptRepository> _reseptRepoMock;
    private readonly Mock<ILagerRepository> _lagerRepoMock;
    private readonly Mock<ILagerTransaksjonRepository> _transaksjonRepoMock;
    private readonly Mock<IArtikkelRepository> _artikkelRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public ProduksjonTests()
    {
        _ordreRepoMock = new Mock<IProduksjonsOrdreRepository>();
        _reseptRepoMock = new Mock<IReseptRepository>();
        _lagerRepoMock = new Mock<ILagerRepository>();
        _transaksjonRepoMock = new Mock<ILagerTransaksjonRepository>();
        _artikkelRepoMock = new Mock<IArtikkelRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    #region CreateProduksjonsOrdreHandler

    [Fact]
    public async Task CreateProduksjonsOrdreHandler_HappyPath_CreatesOrdre()
    {
        var resept = CreateTestResept(1, "Brødresept");
        var command = new CreateProduksjonsOrdreCommand(
            ReseptId: 1,
            OrdreNr: null,
            PlanlagtDato: DateTime.UtcNow.AddDays(1),
            Kommentar: "Testproduksjon");

        ProduksjonsOrdre? captured = null;
        _ordreRepoMock.Setup(r => r.AddAsync(It.IsAny<ProduksjonsOrdre>(), It.IsAny<CancellationToken>()))
            .Callback<ProduksjonsOrdre, CancellationToken>((o, _) => { captured = o; typeof(BaseEntity).GetProperty("Id")!.SetValue(o, 7); })
            .Returns(Task.CompletedTask);
        _ordreRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProduksjonsOrdre>());
        _reseptRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(resept);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateProduksjonsOrdreHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(7, result);
        Assert.NotNull(captured);
        Assert.Equal(ProdOrdreStatus.Planlagt, captured.Status);
        Assert.StartsWith("FG-", captured.FerdigvareLotNr);
        Assert.StartsWith("PROD-", captured.OrdreNr);
        Assert.Equal(1, captured.ReseptId);
    }

    [Fact]
    public async Task CreateProduksjonsOrdreHandler_WithOrdreNr_UsesProvidedOrdreNr()
    {
        var resept = CreateTestResept(1, "Brødresept");
        var command = new CreateProduksjonsOrdreCommand(
            ReseptId: 1,
            OrdreNr: "PO-2026-001",
            PlanlagtDato: DateTime.UtcNow.AddDays(1),
            Kommentar: null);

        ProduksjonsOrdre? captured = null;
        _ordreRepoMock.Setup(r => r.AddAsync(It.IsAny<ProduksjonsOrdre>(), It.IsAny<CancellationToken>()))
            .Callback<ProduksjonsOrdre, CancellationToken>((o, _) => captured = o)
            .Returns(Task.CompletedTask);
        _reseptRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(resept);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateProduksjonsOrdreHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("PO-2026-001", captured!.OrdreNr);
    }

    [Fact]
    public async Task CreateProduksjonsOrdreHandler_ReseptNotFound_Throws()
    {
        var command = new CreateProduksjonsOrdreCommand(
            ReseptId: 999,
            OrdreNr: null,
            PlanlagtDato: DateTime.UtcNow,
            Kommentar: null);

        _reseptRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Resept?)null);

        var handler = new CreateProduksjonsOrdreHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object, _unitOfWorkMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    #endregion

    #region GetAllProduksjonsOrdreHandler

    [Fact]
    public async Task GetAllProduksjonsOrdreHandler_ReturnsAllOrders()
    {
        var ordre = new List<ProduksjonsOrdre>
        {
            CreateTestOrdre(1, "PO-001", ProdOrdreStatus.Planlagt),
            CreateTestOrdre(2, "PO-002", ProdOrdreStatus.Ferdigmeldt)
        };

        _ordreRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ordre);

        var handler = new GetAllProduksjonsOrdreHandler(_ordreRepoMock.Object);

        var result = await handler.Handle(new GetAllProduksjonsOrdreQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllProduksjonsOrdreHandler_EmptyList_ReturnsEmpty()
    {
        _ordreRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProduksjonsOrdre>());

        var handler = new GetAllProduksjonsOrdreHandler(_ordreRepoMock.Object);

        var result = await handler.Handle(new GetAllProduksjonsOrdreQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    #endregion

    #region GetProduksjonsOrdreByIdHandler

    [Fact]
    public async Task GetProduksjonsOrdreByIdHandler_Existing_ReturnsDto()
    {
        var ordre = CreateTestOrdre(5, "PO-005", ProdOrdreStatus.Planlagt);

        _ordreRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(ordre);

        var handler = new GetProduksjonsOrdreByIdHandler(_ordreRepoMock.Object);

        var result = await handler.Handle(new GetProduksjonsOrdreByIdQuery(5), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Ordre.Id);
        Assert.Equal("PO-005", result.Ordre.OrdreNr);
    }

    [Fact]
    public async Task GetProduksjonsOrdreByIdHandler_NonExisting_ReturnsNull()
    {
        _ordreRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProduksjonsOrdre?)null);

        var handler = new GetProduksjonsOrdreByIdHandler(_ordreRepoMock.Object);

        var result = await handler.Handle(new GetProduksjonsOrdreByIdQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region UpdateProduksjonsOrdreStatusHandler

    [Fact]
    public async Task UpdateProduksjonsOrdreStatusHandler_ValidStatus_UpdatesAndReturnsTrue()
    {
        var ordre = CreateTestOrdre(1, "PO-001", ProdOrdreStatus.Planlagt);

        _ordreRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ordre);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateProduksjonsOrdreStatusHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new UpdateProduksjonsOrdreStatusCommand(1, "IProduksjon"),
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal(ProdOrdreStatus.IProduksjon, ordre.Status);
    }

    [Fact]
    public async Task UpdateProduksjonsOrdreStatusHandler_InvalidStatus_ReturnsFalse()
    {
        var ordre = CreateTestOrdre(1, "PO-001", ProdOrdreStatus.Planlagt);

        _ordreRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ordre);

        var handler = new UpdateProduksjonsOrdreStatusHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new UpdateProduksjonsOrdreStatusCommand(1, "UgyldigStatus"),
            CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateProduksjonsOrdreStatusHandler_NonExisting_ReturnsFalse()
    {
        _ordreRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProduksjonsOrdre?)null);

        var handler = new UpdateProduksjonsOrdreStatusHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new UpdateProduksjonsOrdreStatusCommand(999, "IProduksjon"),
            CancellationToken.None);

        Assert.False(result);
    }

    #endregion

    #region FerdigmeldProduksjonsOrdreHandler

    [Fact]
    public async Task FerdigmeldProduksjonsOrdreHandler_HappyPath_SetsFerdigmeldt()
    {
        var resept = CreateTestResept(1, "Brødresept");
        resept.AntallPortjoner = 10;
        var ordre = CreateTestOrdre(1, "PO-001", ProdOrdreStatus.IProduksjon);
        ordre.Resept = resept;
        ordre.ReseptId = 1;

        var command = new FerdigmeldProduksjonsOrdreCommand(
            OrdreId: 1,
            AntallProdusert: 50,
            Kommentar: "Bra resultat",
            UtfortAv: "Kari",
            Forbruk: new List<FerdigmeldForbrukLinjeCommand>
            {
                new(ArtikkelId: 10, LotNr: "LOT-001", MengdeBrukt: 25, Enhet: "kg",
                    Overstyrt: false, Kommentar: null)
            });

        _ordreRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ordre);
        _reseptRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(resept);
        _lagerRepoMock.Setup(r => r.GetByArtikkelOgLotAsync(10, "LOT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LagerBeholdning { Mengde = 100 });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new FerdigmeldProduksjonsOrdreHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object,
            _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result);
        Assert.Equal(ProdOrdreStatus.Ferdigmeldt, ordre.Status);
        Assert.Equal(50, ordre.AntallProdusert);
        Assert.Equal("Bra resultat", ordre.Kommentar);
        Assert.Equal("Kari", ordre.UtfortAv);
        Assert.NotNull(ordre.FerdigmeldtDato);

        _transaksjonRepoMock.Verify(r => r.AddAsync(
            It.Is<LagerTransaksjon>(t => t.Type == TransaksjonsType.ProduksjonUttak && t.LotNr == "LOT-001"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FerdigmeldProduksjonsOrdreHandler_AlreadyFerdigmeldt_Throws()
    {
        var ordre = CreateTestOrdre(1, "PO-001", ProdOrdreStatus.Ferdigmeldt);

        _ordreRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ordre);

        var handler = new FerdigmeldProduksjonsOrdreHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object,
            _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var command = new FerdigmeldProduksjonsOrdreCommand(
            OrdreId: 1, AntallProdusert: 10, Kommentar: null, UtfortAv: "Ola",
            Forbruk: null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task FerdigmeldProduksjonsOrdreHandler_Kansellert_Throws()
    {
        var ordre = CreateTestOrdre(1, "PO-001", ProdOrdreStatus.Kansellert);

        _ordreRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ordre);

        var handler = new FerdigmeldProduksjonsOrdreHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object,
            _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var command = new FerdigmeldProduksjonsOrdreCommand(
            OrdreId: 1, AntallProdusert: 10, Kommentar: null, UtfortAv: "Ola",
            Forbruk: null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task FerdigmeldProduksjonsOrdreHandler_OrdreNotFound_Throws()
    {
        _ordreRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProduksjonsOrdre?)null);

        var handler = new FerdigmeldProduksjonsOrdreHandler(
            _ordreRepoMock.Object, _reseptRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object,
            _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var command = new FerdigmeldProduksjonsOrdreCommand(
            OrdreId: 999, AntallProdusert: 10, Kommentar: null, UtfortAv: "Ola",
            Forbruk: null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    #endregion

    // --- Helpers ---
    private static ProduksjonsOrdre CreateTestOrdre(int id, string ordreNr, ProdOrdreStatus status)
    {
        var ordre = new ProduksjonsOrdre
        {
            ReseptId = 1,
            OrdreNr = ordreNr,
            PlanlagtDato = DateTime.UtcNow,
            Status = status,
            FerdigvareLotNr = "FG-TEST",
            OpprettetDato = DateTime.UtcNow,
            Forbruk = new List<ProdOrdreForbruk>()
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ordre, id);
        return ordre;
    }

    private static Resept CreateTestResept(int id, string navn)
    {
        var resept = new Resept
        {
            Navn = navn,
            AntallPortjoner = 1,
            FerdigvareId = 10,
            OpprettetDato = DateTime.UtcNow
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(resept, id);
        return resept;
    }
}
