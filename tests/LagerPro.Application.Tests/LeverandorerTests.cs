using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Leverandorer;
using LagerPro.Application.Features.Leverandorer.Commands;
using LagerPro.Application.Features.Leverandorer.Queries.GetAllLeverandorer;
using LagerPro.Application.Features.Leverandorer.Queries.GetLeverandorById;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using Moq;

namespace LagerPro.Application.Tests;

public class LeverandorerTests
{
    private readonly Mock<ILeverandorRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public LeverandorerTests()
    {
        _repositoryMock = new Mock<ILeverandorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    #region CreateLeverandorHandler

    [Fact]
    public async Task CreateLeverandorHandler_HappyPath_SetsFieldsAndSaves()
    {
        var command = new CreateLeverandorCommand(
            Navn: "Mills AS",
            Kontaktperson: "Kari Sørensen",
            Telefon: "98765432",
            Epost: "kari@mills.no",
            Adresse: "Industrivegen 10",
            Postnr: "6840",
            Poststed: "Svelgen",
            OrgNr: "987654321",
            Kommentar: "Mel-leverandør");

        Leverandor? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Leverandor>(), It.IsAny<CancellationToken>()))
            .Callback<Leverandor, CancellationToken>((l, _) => captured = l)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateLeverandorHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Mills AS", captured!.Navn);
        Assert.Equal("Kari Sørensen", captured.Kontaktperson);
        Assert.Equal("98765432", captured.Telefon);
        Assert.True(captured.Aktiv);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Leverandor>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateLeverandorHandler_ReturnsId()
    {
        var command = new CreateLeverandorCommand(
            Navn: "Test Lev", null, null, null, null, null, null, null, null);

        Leverandor? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Leverandor>(), It.IsAny<CancellationToken>()))
            .Callback<Leverandor, CancellationToken>((l, _) => { captured = l; typeof(BaseEntity).GetProperty("Id")!.SetValue(l, 99); })
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateLeverandorHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(99, result);
    }

    #endregion

    #region GetAllLeverandorerHandler

    [Fact]
    public async Task GetAllLeverandorerHandler_ReturnsAllLeverandorer()
    {
        var leverandorer = new List<Leverandor>
        {
            CreateTestLeverandor(1, "Lev A"),
            CreateTestLeverandor(2, "Lev B")
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(leverandorer);

        var handler = new GetAllLeverandorerHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllLeverandorerQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Lev A", result[0].Navn);
        Assert.Equal("Lev B", result[1].Navn);
    }

    [Fact]
    public async Task GetAllLeverandorerHandler_EmptyList_ReturnsEmpty()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Leverandor>());

        var handler = new GetAllLeverandorerHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllLeverandorerQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    #endregion

    #region GetLeverandorByIdHandler

    [Fact]
    public async Task GetLeverandorByIdHandler_Existing_ReturnsLeverandorDto()
    {
        var lev = CreateTestLeverandor(7, "Spesial Lev");

        _repositoryMock.Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(lev);

        var handler = new GetLeverandorByIdHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetLeverandorByIdQuery(7), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(7, result!.Id);
        Assert.Equal("Spesial Lev", result.Navn);
    }

    [Fact]
    public async Task GetLeverandorByIdHandler_NonExisting_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Leverandor?)null);

        var handler = new GetLeverandorByIdHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetLeverandorByIdQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region UpdateLeverandorHandler

    [Fact]
    public async Task UpdateLeverandorHandler_Existing_UpdatesAndReturnsTrue()
    {
        var lev = CreateTestLeverandor(1, "Gammel Navn");

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(lev);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateLeverandorHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var command = new UpdateLeverandorCommand(
            Id: 1,
            Navn: "Ny Navn",
            Kontaktperson: "Ny Kontakt",
            Telefon: "11111111",
            Epost: "ny@epost.no",
            Adresse: "Ny Gate 5",
            Postnr: "6800",
            Poststed: "Nyby",
            OrgNr: "NEW123",
            Kommentar: "Oppdatert",
            Aktiv: true);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.Equal("Ny Navn", lev.Navn);
        Assert.Equal("Ny Kontakt", lev.Kontaktperson);
        Assert.Equal("11111111", lev.Telefon);
        _repositoryMock.Verify(r => r.Update(lev), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLeverandorHandler_NonExisting_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Leverandor?)null);

        var handler = new UpdateLeverandorHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var command = new UpdateLeverandorCommand(999, "Navn", null, null, null, null, null, null, null, null, true);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Leverandor>()), Times.Never);
    }

    #endregion

    #region DeleteLeverandorHandler (soft-delete)

    [Fact]
    public async Task DeleteLeverandorHandler_Existing_SoftDeletesAndReturnsTrue()
    {
        var lev = CreateTestLeverandor(3, "Deaktiver Meg");

        _repositoryMock.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(lev);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteLeverandorHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new DeleteLeverandorCommand(3), CancellationToken.None);

        Assert.True(result);
        Assert.False(lev.Aktiv); // soft-delete: deaktiverer istedenfor å slette
        _repositoryMock.Verify(r => r.Update(lev), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteLeverandorHandler_NonExisting_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Leverandor?)null);

        var handler = new DeleteLeverandorHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new DeleteLeverandorCommand(999), CancellationToken.None);

        Assert.False(result);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Leverandor>()), Times.Never);
    }

    #endregion

    // --- Helper ---
    private static Leverandor CreateTestLeverandor(int id, string navn)
    {
        var lev = new Leverandor { Navn = navn, Aktiv = true };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(lev, id);
        return lev;
    }
}
