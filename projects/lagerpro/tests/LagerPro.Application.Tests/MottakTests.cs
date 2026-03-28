using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Mottak.Commands.CreateMottak;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakStatus;
using LagerPro.Application.Features.Mottak.Queries.GetAllMottak;
using LagerPro.Application.Features.Mottak.Queries.GetMottakById;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;
using Moq;

namespace LagerPro.Application.Tests;

public class MottakTests
{
    private readonly Mock<IMottakRepository> _mottakRepoMock;
    private readonly Mock<IArtikkelRepository> _artikkelRepoMock;
    private readonly Mock<ILagerRepository> _lagerRepoMock;
    private readonly Mock<ILagerTransaksjonRepository> _transaksjonRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public MottakTests()
    {
        _mottakRepoMock = new Mock<IMottakRepository>();
        _artikkelRepoMock = new Mock<IArtikkelRepository>();
        _lagerRepoMock = new Mock<ILagerRepository>();
        _transaksjonRepoMock = new Mock<ILagerTransaksjonRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    #region GetAllMottakHandler

    [Fact]
    public async Task GetAllMottakHandler_ReturnsAllMottak()
    {
        var mottakList = new List<Mottak>
        {
            CreateTestMottak(1, MottakStatus.Registrert),
            CreateTestMottak(2, MottakStatus.Godkjent)
        };

        _mottakRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mottakList);

        var handler = new GetAllMottakHandler(_mottakRepoMock.Object);

        var result = await handler.Handle(new GetAllMottakQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllMottakHandler_EmptyList_ReturnsEmpty()
    {
        _mottakRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Mottak>());

        var handler = new GetAllMottakHandler(_mottakRepoMock.Object);

        var result = await handler.Handle(new GetAllMottakQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    #endregion

    #region GetMottakByIdHandler

    [Fact]
    public async Task GetMottakByIdHandler_Existing_ReturnsMottakDto()
    {
        var mottak = CreateTestMottak(5, MottakStatus.Registrert);

        _mottakRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(mottak);

        var handler = new GetMottakByIdHandler(_mottakRepoMock.Object);

        var result = await handler.Handle(new GetMottakByIdQuery(5), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
    }

    [Fact]
    public async Task GetMottakByIdHandler_NonExisting_ReturnsNull()
    {
        _mottakRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mottak?)null);

        var handler = new GetMottakByIdHandler(_mottakRepoMock.Object);

        var result = await handler.Handle(new GetMottakByIdQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region UpdateMottakStatusHandler

    [Fact]
    public async Task UpdateMottakStatusHandler_ValidStatus_UpdatesAndReturnsTrue()
    {
        var mottak = CreateTestMottak(1, MottakStatus.Registrert);
        mottak.MottattAv = "Ola";

        _mottakRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(mottak);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateMottakStatusHandler(
            _mottakRepoMock.Object, _lagerRepoMock.Object,
            _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new UpdateMottakStatusCommand(1, "Mottatt"), CancellationToken.None);

        Assert.True(result);
        Assert.Equal(MottakStatus.Mottatt, mottak.Status);
        _mottakRepoMock.Verify(r => r.UpdateAsync(mottak, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMottakStatusHandler_InvalidStatus_ReturnsFalse()
    {
        var mottak = CreateTestMottak(1, MottakStatus.Registrert);

        _mottakRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(mottak);

        var handler = new UpdateMottakStatusHandler(
            _mottakRepoMock.Object, _lagerRepoMock.Object,
            _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new UpdateMottakStatusCommand(1, "UgyldigStatus"), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateMottakStatusHandler_NonExisting_ReturnsFalse()
    {
        _mottakRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mottak?)null);

        var handler = new UpdateMottakStatusHandler(
            _mottakRepoMock.Object, _lagerRepoMock.Object,
            _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new UpdateMottakStatusCommand(999, "Godkjent"), CancellationToken.None);

        Assert.False(result);
    }

    #endregion

    #region CreateMottakHandler

    [Fact]
    public async Task CreateMottakHandler_HappyPath_CreatesMottakWithCorrectLines()
    {
        var artikkel = new Artikkel { Navn = "Hvetemel", Enhet = "kg" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(artikkel, 10);

        var command = new CreateMottakCommand(
            LeverandorId: 1,
            MottaksDato: DateTime.UtcNow,
            Referanse: "REF-001",
            Kommentar: "Test",
            MottattAv: "Steve",
            Linjer: new List<MottakLinjeCommand>
            {
                new(ArtikkelId: 10, LotNr: "LOT-001", Mengde: 100, Enhet: "kg",
                    BestForDato: null, Temperatur: null, Strekkode: null, Avvik: null,
                    Kommentar: null, Godkjent: false),
                new(ArtikkelId: 10, LotNr: "LOT-002", Mengde: 50, Enhet: "kg",
                    BestForDato: null, Temperatur: null, Strekkode: null, Avvik: "Skade på sekk",
                    Kommentar: null, Godkjent: false),
            });

        Mottak? capturedMottak = null;
        _mottakRepoMock.Setup(r => r.AddAsync(It.IsAny<Mottak>(), It.IsAny<CancellationToken>()))
            .Callback<Mottak, CancellationToken>((m, _) => { capturedMottak = m; typeof(BaseEntity).GetProperty("Id")!.SetValue(m, 99); })
            .Returns(Task.CompletedTask);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(artikkel);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateMottakHandler(
            _mottakRepoMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(99, result);
        Assert.NotNull(capturedMottak);
        Assert.Equal(2, capturedMottak!.Linjer.Count);
        // Linje UTEN avvik (LOT-001) skal godkjennes
        Assert.Single(capturedMottak.Linjer, l => l.Godkjent && l.LotNr == "LOT-001");
        // Linje MED avvik (LOT-002) skal IKKE godkjennes
        Assert.Single(capturedMottak.Linjer, l => !l.Godkjent && l.LotNr == "LOT-002");
    }

    [Fact]
    public async Task CreateMottakHandler_StoresCorrectStatus()
    {
        var command = new CreateMottakCommand(
            LeverandorId: 1,
            MottaksDato: DateTime.UtcNow,
            Referanse: null,
            Kommentar: null,
            MottattAv: "Ola",
            Linjer: new List<MottakLinjeCommand>());

        Mottak? captured = null;
        _mottakRepoMock.Setup(r => r.AddAsync(It.IsAny<Mottak>(), It.IsAny<CancellationToken>()))
            .Callback<Mottak, CancellationToken>((m, _) => captured = m)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateMottakHandler(
            _mottakRepoMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(MottakStatus.Registrert, captured!.Status);
        Assert.Equal("Ola", captured.MottattAv);
    }

    #endregion

    // --- Helper ---
    private static Mottak CreateTestMottak(int id, MottakStatus status)
    {
        var mottak = new Mottak
        {
            LeverandorId = 1,
            MottaksDato = DateTime.UtcNow,
            Status = status,
            MottattAv = "Test User",
            OpprettetDato = DateTime.UtcNow,
            Linjer = new List<MottakLinje>()
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(mottak, id);
        return mottak;
    }
}
