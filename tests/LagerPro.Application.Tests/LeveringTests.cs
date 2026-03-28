using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Levering.Commands.CreateLevering;
using LagerPro.Application.Features.Levering.Commands.UpdateLeveringStatus;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;
using Moq;

namespace LagerPro.Application.Tests;

public class LeveringTests
{
    private readonly Mock<ILeveringRepository> _leveringRepoMock;
    private readonly Mock<IKundeRepository> _kundeRepoMock;
    private readonly Mock<ILagerRepository> _lagerRepoMock;
    private readonly Mock<ILagerTransaksjonRepository> _transaksjonRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public LeveringTests()
    {
        _leveringRepoMock = new Mock<ILeveringRepository>();
        _kundeRepoMock = new Mock<IKundeRepository>();
        _lagerRepoMock = new Mock<ILagerRepository>();
        _transaksjonRepoMock = new Mock<ILagerTransaksjonRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    #region CreateLeveringHandler

    [Fact]
    public async Task CreateLeveringHandler_HappyPath_CreatesLeveringAndDeductsLager()
    {
        var kunde = CreateTestKunde(5, "Brenneriet AS");
        var command = new CreateLeveringCommand(
            KundeId: 5,
            LeveringsDato: DateTime.UtcNow.AddDays(1),
            Referanse: "ORD-001",
            FraktBrev: "FB-123",
            Kommentar: "Haste",
            LevertAv: "Ola",
            Linjer: new List<LeveringLinjeCommand>
            {
                new(ArtikkelId: 10, LotNr: "LOT-001", Mengde: 5, Enhet: "kg", Kommentar: null),
            });

        Levering? captured = null;
        _leveringRepoMock.Setup(r => r.AddAsync(It.IsAny<Levering>(), It.IsAny<CancellationToken>()))
            .Callback<Levering, CancellationToken>((l, _) => { captured = l; typeof(BaseEntity).GetProperty("Id")!.SetValue(l, 12); })
            .Returns(Task.CompletedTask);
        _kundeRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(kunde);
        _lagerRepoMock.Setup(r => r.GetByArtikkelOgLotAsync(10, "LOT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LagerBeholdning { Mengde = 100 });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateLeveringHandler(
            _leveringRepoMock.Object, _kundeRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(12, result);
        Assert.NotNull(captured);
        Assert.Equal(LeveringStatus.Planlagt, captured!.Status);
        Assert.Single(captured.Linjer);

        // Lager skal trekkes (GetByArtikkelOgLotAsync kalles 2x: pre-check + faktisk trekk)
        _lagerRepoMock.Verify(r => r.GetByArtikkelOgLotAsync(10, "LOT-001", It.IsAny<CancellationToken>()), Times.Exactly(2));
        _transaksjonRepoMock.Verify(r => r.AddAsync(
            It.Is<LagerTransaksjon>(t => t.Type == TransaksjonsType.Levering && t.Mengde == 5 && t.LotNr == "LOT-001"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateLeveringHandler_KundeNotFound_Throws()
    {
        var command = new CreateLeveringCommand(
            KundeId: 999,
            LeveringsDato: DateTime.UtcNow,
            Referanse: null,
            FraktBrev: null,
            Kommentar: null,
            LevertAv: "Ola",
            Linjer: new List<LeveringLinjeCommand>());

        _kundeRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Kunde?)null);

        var handler = new CreateLeveringHandler(
            _leveringRepoMock.Object, _kundeRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateLeveringHandler_MultipleLinjer_DeductsAllFromLager()
    {
        var kunde = CreateTestKunde(1, "Test Kunde");
        var command = new CreateLeveringCommand(
            KundeId: 1,
            LeveringsDato: DateTime.UtcNow,
            Referanse: null,
            FraktBrev: null,
            Kommentar: null,
            LevertAv: "Steve",
            Linjer: new List<LeveringLinjeCommand>
            {
                new(ArtikkelId: 10, LotNr: "LOT-A", Mengde: 5, Enhet: "kg", Kommentar: null),
                new(ArtikkelId: 11, LotNr: "LOT-B", Mengde: 3, Enhet: "liter", Kommentar: null),
            });

        Levering? captured = null;
        _leveringRepoMock.Setup(r => r.AddAsync(It.IsAny<Levering>(), It.IsAny<CancellationToken>()))
            .Callback<Levering, CancellationToken>((l, _) => captured = l)
            .Returns(Task.CompletedTask);
        _kundeRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(kunde);
        _lagerRepoMock.Setup(r => r.GetByArtikkelOgLotAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LagerBeholdning { Mengde = 100 });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateLeveringHandler(
            _leveringRepoMock.Object, _kundeRepoMock.Object,
            _lagerRepoMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(2, captured!.Linjer.Count);
        // Hver linje kaller GetByArtikkelOgLotAsync 2x (pre-check + trek)
        _lagerRepoMock.Verify(r => r.GetByArtikkelOgLotAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
        // Én transaksjon per linje
        _transaksjonRepoMock.Verify(r => r.AddAsync(
            It.Is<LagerTransaksjon>(t => t.ArtikkelId == 10 && t.LotNr == "LOT-A"),
            It.IsAny<CancellationToken>()), Times.Once);
        _transaksjonRepoMock.Verify(r => r.AddAsync(
            It.Is<LagerTransaksjon>(t => t.ArtikkelId == 11 && t.LotNr == "LOT-B"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateLeveringStatusHandler

    [Fact]
    public async Task UpdateLeveringStatusHandler_PlanlagtToSendt_UpdatesStatus()
    {
        var levering = CreateTestLevering(1, LeveringStatus.Planlagt);

        _leveringRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(levering);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateLeveringStatusHandler(
            _leveringRepoMock.Object, _lagerRepoMock.Object,
            _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new UpdateLeveringStatusCommand(1, "Sendt"),
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal(LeveringStatus.Sendt, levering.Status);
    }

    [Fact]
    public async Task UpdateLeveringStatusHandler_PlanlagtToLagt_LogsDeliveryTransaction()
    {
        var levering = CreateTestLevering(1, LeveringStatus.Planlagt);
        levering.LevertAv = "Ola";
        levering.Linjer.Add(new LeveringLinje
        {
            ArtikkelId = 10,
            LotNr = "LOT-001",
            Mengde = 5,
            Enhet = "kg"
        });

        _leveringRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(levering);
        _lagerRepoMock.Setup(r => r.GetByArtikkelOgLotAsync(10, "LOT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LagerBeholdning { Mengde = 95 });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateLeveringStatusHandler(
            _leveringRepoMock.Object, _lagerRepoMock.Object,
            _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new UpdateLeveringStatusCommand(1, "Levert"),
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal(LeveringStatus.Levert, levering.Status);

        // Skal logge transaksjon (bekreftelse, ikke ny reduksjon)
        _transaksjonRepoMock.Verify(r => r.AddAsync(
            It.Is<LagerTransaksjon>(t => t.Type == TransaksjonsType.LeveringBekreftet && t.Mengde == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLeveringStatusHandler_InvalidStatus_ReturnsFalse()
    {
        var levering = CreateTestLevering(1, LeveringStatus.Planlagt);

        _leveringRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(levering);

        var handler = new UpdateLeveringStatusHandler(
            _leveringRepoMock.Object, _lagerRepoMock.Object,
            _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new UpdateLeveringStatusCommand(1, "UgyldigStatus"),
            CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateLeveringStatusHandler_NonExisting_ReturnsFalse()
    {
        _leveringRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Levering?)null);

        var handler = new UpdateLeveringStatusHandler(
            _leveringRepoMock.Object, _lagerRepoMock.Object,
            _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new UpdateLeveringStatusCommand(999, "Sendt"),
            CancellationToken.None);

        Assert.False(result);
    }

    #endregion

    // --- Helpers ---
    private static Kunde CreateTestKunde(int id, string navn)
    {
        var kunde = new Kunde { Navn = navn, Aktiv = true };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(kunde, id);
        return kunde;
    }

    private static Levering CreateTestLevering(int id, LeveringStatus status)
    {
        var levering = new Levering
        {
            KundeId = 1,
            LeveringsDato = DateTime.UtcNow,
            Status = status,
            OpprettetDato = DateTime.UtcNow,
            Linjer = new List<LeveringLinje>()
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(levering, id);
        return levering;
    }
}
