using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Kunder;
using LagerPro.Application.Features.Kunder.Commands;
using LagerPro.Application.Features.Kunder.Queries.GetAllKunder;
using LagerPro.Application.Features.Kunder.Queries.GetKundeById;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using Moq;

namespace LagerPro.Application.Tests;

public class KunderTests
{
    private readonly Mock<IKundeRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public KunderTests()
    {
        _repositoryMock = new Mock<IKundeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    #region CreateKundeHandler

    [Fact]
    public async Task CreateKundeHandler_HappyPath_SetsFieldsAndSaves()
    {
        var command = new CreateKundeCommand(
            Navn: "Brenneriet AS",
            Kontaktperson: "Ola Nordmann",
            Telefon: "12345678",
            Epost: "ola@brenneriet.no",
            Adresse: "Storgata 1",
            Postnr: "6840",
            Poststed: "Svelgen",
            OrgNr: "123456789",
            Kommentar: "Faste kunde");

        Kunde? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Kunde>(), It.IsAny<CancellationToken>()))
            .Callback<Kunde, CancellationToken>((k, _) => captured = k)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Brenneriet AS", captured!.Navn);
        Assert.Equal("Ola Nordmann", captured.Kontaktperson);
        Assert.Equal("12345678", captured.Telefon);
        Assert.True(captured.Aktiv);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Kunde>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateKundeHandler_ReturnsId()
    {
        var command = new CreateKundeCommand(
            Navn: "Test Kunde", null, null, null, null, null, null, null, null);

        Kunde? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Kunde>(), It.IsAny<CancellationToken>()))
            .Callback<Kunde, CancellationToken>((k, _) => { captured = k; typeof(BaseEntity).GetProperty("Id")!.SetValue(k, 42); })
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task CreateKundeHandler_DuplicateOrgNr_ThrowsInvalidOperationException()
    {
        var command = new CreateKundeCommand(
            Navn: "Ny Kunde", null, null, null, null, null, null, "987654321", null);

        _repositoryMock.Setup(r => r.GetByOrgNrAsync("987654321", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Kunde { Navn = "Eksisterande Kunde", OrgNr = "987654321" });

        var handler = new CreateKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Contains("987654321", ex.Message);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Kunde>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateKundeHandler_OrgNrNull_BypassesDuplicateCheck()
    {
        var command = new CreateKundeCommand(
            Navn: "Kunde utan orgnr", null, null, null, null, null, null, null, null);

        Kunde? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Kunde>(), It.IsAny<CancellationToken>()))
            .Callback<Kunde, CancellationToken>((k, _) => captured = k)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("Kunde utan orgnr", captured!.Navn);
        _repositoryMock.Verify(r => r.GetByOrgNrAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetAllKunderHandler

    [Fact]
    public async Task GetAllKunderHandler_ReturnsAllKunder()
    {
        var kunder = new List<Kunde>
        {
            CreateTestKunde(1, "Kunde A"),
            CreateTestKunde(2, "Kunde B")
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(kunder);

        var handler = new GetAllKunderHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllKunderQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Kunde A", result[0].Navn);
        Assert.Equal("Kunde B", result[1].Navn);
    }

    [Fact]
    public async Task GetAllKunderHandler_EmptyList_ReturnsEmpty()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Kunde>());

        var handler = new GetAllKunderHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllKunderQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    #endregion

    #region GetKundeByIdHandler

    [Fact]
    public async Task GetKundeByIdHandler_Existing_ReturnsKundeDto()
    {
        var kunde = CreateTestKunde(5, "Spesial Kunde");

        _repositoryMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(kunde);

        var handler = new GetKundeByIdHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetKundeByIdQuery(5), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
        Assert.Equal("Spesial Kunde", result.Navn);
    }

    [Fact]
    public async Task GetKundeByIdHandler_NonExisting_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Kunde?)null);

        var handler = new GetKundeByIdHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetKundeByIdQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region UpdateKundeHandler

    [Fact]
    public async Task UpdateKundeHandler_Existing_UpdatesAndReturnsTrue()
    {
        var kunde = CreateTestKunde(1, "Gammel Navn");

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(kunde);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var command = new UpdateKundeCommand(
            Id: 1,
            Navn: "Ny Navn",
            Kontaktperson: "Ny Kontakt",
            Telefon: "99999999",
            Epost: "ny@epost.no",
            Adresse: "Ny Gate 5",
            Postnr: "6800",
            Poststed: "Nyby",
            OrgNr: "NEW123",
            Kommentar: "Oppdatert",
            Aktiv: true);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.Equal("Ny Navn", kunde.Navn);
        Assert.Equal("Ny Kontakt", kunde.Kontaktperson);
        Assert.Equal("99999999", kunde.Telefon);
        _repositoryMock.Verify(r => r.Update(kunde), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateKundeHandler_NonExisting_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Kunde?)null);

        var handler = new UpdateKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var command = new UpdateKundeCommand(999, "Navn", null, null, null, null, null, null, null, null, true);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Kunde>()), Times.Never);
    }

    [Fact]
    public async Task UpdateKundeHandler_DuplicateOrgNr_ThrowsInvalidOperationException()
    {
        var kunde = CreateTestKunde(1, "Min Kunde");
        kunde.OrgNr = "111111111";

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(kunde);
        _repositoryMock.Setup(r => r.GetByOrgNrAsync("222222222", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Kunde { Navn = "Annen Kunde", OrgNr = "222222222" });

        var handler = new UpdateKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var command = new UpdateKundeCommand(
            Id: 1, Navn: "Min Kunde", Kontaktperson: null, Telefon: null, Epost: null,
            Adresse: null, Postnr: null, Poststed: null, OrgNr: "222222222", Kommentar: null, Aktiv: true);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Contains("222222222", ex.Message);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Kunde>()), Times.Never);
    }

    [Fact]
    public async Task UpdateKundeHandler_SameOrgNrAsSelf_DoesNotThrow()
    {
        var kunde = CreateTestKunde(1, "Min Kunde");
        kunde.OrgNr = "555555555";

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(kunde);
        _repositoryMock.Setup(r => r.GetByOrgNrAsync("555555555", It.IsAny<CancellationToken>()))
            .ReturnsAsync(kunde);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var command = new UpdateKundeCommand(
            Id: 1, Navn: "Min Kunde", Kontaktperson: null, Telefon: null, Epost: null,
            Adresse: null, Postnr: null, Poststed: null, OrgNr: "555555555", Kommentar: null, Aktiv: true);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    #endregion

    #region DeleteKundeHandler (soft-delete)

    [Fact]
    public async Task DeleteKundeHandler_Existing_SoftDeletesAndReturnsTrue()
    {
        var kunde = CreateTestKunde(3, "Deaktiver Meg");

        _repositoryMock.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(kunde);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new DeleteKundeCommand(3), CancellationToken.None);

        Assert.True(result);
        Assert.False(kunde.Aktiv); // soft-delete: deaktiverer istedenfor å slette
        _repositoryMock.Verify(r => r.Update(kunde), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteKundeHandler_NonExisting_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Kunde?)null);

        var handler = new DeleteKundeHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new DeleteKundeCommand(999), CancellationToken.None);

        Assert.False(result);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Kunde>()), Times.Never);
    }

    #endregion

    // --- Helper ---
    private static Kunde CreateTestKunde(int id, string navn)
    {
        var kunde = new Kunde { Navn = navn, Aktiv = true };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(kunde, id);
        return kunde;
    }
}
