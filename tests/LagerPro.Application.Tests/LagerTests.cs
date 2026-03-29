using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Lager.Commands.JusterLager;
using LagerPro.Application.Features.Lager.Queries.GetAllLagerBeholdning;
using LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByArtikkel;
using LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByLotNr;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using Moq;

namespace LagerPro.Application.Tests;

public class LagerTests
{
    private readonly Mock<ILagerRepository> _repositoryMock;
    private readonly Mock<ILagerTransaksjonRepository> _transaksjonRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public LagerTests()
    {
        _repositoryMock = new Mock<ILagerRepository>();
    }

    #region GetAllLagerBeholdningHandler

    [Fact]
    public async Task GetAllLagerBeholdningHandler_ReturnsGroupedByArtikkel()
    {
        var beholdninger = new List<LagerBeholdning>
        {
            CreateTestBeholdning(1, 10, "LOT-001", 100, "kg"),
            CreateTestBeholdning(2, 10, "LOT-002", 50, "kg"),
            CreateTestBeholdning(3, 20, "LOT-003", 200, "liter"),
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(beholdninger);

        var handler = new GetAllLagerBeholdningHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllLagerBeholdningQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        var artikkel10 = result.First(r => r.ArtikkelId == 10);
        Assert.Equal(150, artikkel10.TotalMengde); // 100 + 50
        Assert.Equal(2, artikkel10.AntallLots);
        Assert.Equal(2, artikkel10.Detaljer.Count);
    }

    [Fact]
    public async Task GetAllLagerBeholdningHandler_EmptyList_ReturnsEmpty()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LagerBeholdning>());

        var handler = new GetAllLagerBeholdningHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllLagerBeholdningQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllLagerBeholdningHandler_SingleLot_GroupedCorrectly()
    {
        var beholdninger = new List<LagerBeholdning>
        {
            CreateTestBeholdning(1, 10, "LOT-001", 100, "kg")
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(beholdninger);

        var handler = new GetAllLagerBeholdningHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllLagerBeholdningQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(100, result[0].TotalMengde);
        Assert.Equal(1, result[0].AntallLots);
    }

    #endregion

    #region GetLagerBeholdningByArtikkelHandler

    [Fact]
    public async Task GetLagerBeholdningByArtikkelHandler_ReturnsLotsForArtikkel()
    {
        var beholdninger = new List<LagerBeholdning>
        {
            CreateTestBeholdning(1, 10, "LOT-A", 100, "kg"),
            CreateTestBeholdning(2, 10, "LOT-B", 75, "kg")
        };

        _repositoryMock.Setup(r => r.GetByArtikkelAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beholdninger);

        var handler = new GetLagerBeholdningByArtikkelHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetLagerBeholdningByArtikkelQuery(10), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("LOT-A", result[0].LotNr);
        Assert.Equal("LOT-B", result[1].LotNr);
    }

    [Fact]
    public async Task GetLagerBeholdningByArtikkelHandler_NoLots_ReturnsEmpty()
    {
        _repositoryMock.Setup(r => r.GetByArtikkelAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LagerBeholdning>());

        var handler = new GetLagerBeholdningByArtikkelHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetLagerBeholdningByArtikkelQuery(999), CancellationToken.None);

        Assert.Empty(result);
    }

    #endregion

    #region GetLagerBeholdningByLotNrHandler

    [Fact]
    public async Task GetLagerBeholdningByLotNrHandler_Existing_ReturnsBeholdningDto()
    {
        var beholdning = CreateTestBeholdning(5, 10, "LOT-XYZ", 250, "kg");

        _repositoryMock.Setup(r => r.GetByLotNrAsync("LOT-XYZ", It.IsAny<CancellationToken>()))
            .ReturnsAsync(beholdning);

        var handler = new GetLagerBeholdningByLotNrHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetLagerBeholdningByLotNrQuery("LOT-XYZ"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("LOT-XYZ", result!.LotNr);
        Assert.Equal(250, result.Mengde);
        Assert.Equal(10, result.ArtikkelId);
    }

    [Fact]
    public async Task GetLagerBeholdningByLotNrHandler_NonExisting_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByLotNrAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((LagerBeholdning?)null);

        var handler = new GetLagerBeholdningByLotNrHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetLagerBeholdningByLotNrQuery("NONEXISTENT"), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region JusterLagerHandler

    [Fact]
    public async Task JusterLagerHandler_GyldigBeholdning_JustererOgLagrerTransaksjon()
    {
        // Arrange
        var beholdning = CreateTestBeholdning(1, 10, "LOT-001", 50m, "KG");

        _repositoryMock.Setup(r => r.GetByArtikkelOgLotAsync(10, "LOT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(beholdning);
        _transaksjonRepoMock.Setup(r => r.AddAsync(It.IsAny<LagerTransaksjon>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new JusterLagerHandler(_repositoryMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(
            new JusterLagerCommand(10, "LOT-001", 45.5m, "Varetelling", "Martin"),
            CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(45.5m, beholdning.Mengde);
        _transaksjonRepoMock.Verify(
            r => r.AddAsync(It.Is<LagerTransaksjon>(t =>
                t.ArtikkelId == 10 &&
                t.LotNr == "LOT-001" &&
                t.Mengde == -4.5m &&
                t.Type == Domain.Enums.TransaksjonsType.Justering),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task JusterLagerHandler_IkkeFunnet_KasterException()
    {
        _repositoryMock.Setup(r => r.GetByArtikkelOgLotAsync(99, "LOT-X", It.IsAny<CancellationToken>()))
            .ReturnsAsync((LagerBeholdning?)null);

        var handler = new JusterLagerHandler(_repositoryMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new JusterLagerCommand(99, "LOT-X", 10m, null, null), CancellationToken.None));
    }

    [Fact]
    public async Task JusterLagerHandler_Nullstill_LagrerNegativTransaksjon()
    {
        var beholdning = CreateTestBeholdning(1, 10, "LOT-001", 100m, "KG");

        _repositoryMock.Setup(r => r.GetByArtikkelOgLotAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beholdning);
        _transaksjonRepoMock.Setup(r => r.AddAsync(It.IsAny<LagerTransaksjon>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new JusterLagerHandler(_repositoryMock.Object, _transaksjonRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new JusterLagerCommand(10, "LOT-001", 0m, "Nullstill", "Admin"),
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal(0m, beholdning.Mengde);
        _transaksjonRepoMock.Verify(r => r.AddAsync(
            It.Is<LagerTransaksjon>(t => t.Mengde == -100m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    // --- Helper ---
    private static LagerBeholdning CreateTestBeholdning(int id, int artikkelId, string lotNr, decimal mengde, string enhet)
    {
        var artikkel = new Artikkel
        {
            ArtikkelNr = $"ART-{artikkelId:D3}",
            Navn = $"Artikkel {artikkelId}",
            Enhet = enhet
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(artikkel, artikkelId);

        var beholdning = new LagerBeholdning
        {
            ArtikkelId = artikkelId,
            Artikkel = artikkel,
            LotNr = lotNr,
            Mengde = mengde,
            Enhet = enhet,
            SistOppdatert = DateTime.UtcNow
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(beholdning, id);
        return beholdning;
    }
}
