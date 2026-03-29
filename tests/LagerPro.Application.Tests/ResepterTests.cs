using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Resepter.Commands.CreateResept;
using LagerPro.Application.Features.Resepter.Commands.DeleteResept;
using LagerPro.Application.Features.Resepter.Commands.UpdateResept;
using LagerPro.Application.Features.Resepter.Queries.GetAllResepter;
using LagerPro.Application.Features.Resepter.Queries.GetReseptById;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using Moq;
using CreateLinjeCmd = LagerPro.Application.Features.Resepter.Commands.CreateResept.ReseptLinjeCommand;
using UpdateLinjeCmd = LagerPro.Application.Features.Resepter.Commands.UpdateResept.ReseptLinjeCommand;

namespace LagerPro.Application.Tests;

public class ResepterTests
{
    private readonly Mock<IReseptRepository> _repositoryMock;
    private readonly Mock<IArtikkelRepository> _artikkelRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public ResepterTests()
    {
        _repositoryMock = new Mock<IReseptRepository>();
        _artikkelRepoMock = new Mock<IArtikkelRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    #region GetAllResepterHandler

    [Fact]
    public async Task GetAllResepterHandler_ReturnsAllResepter()
    {
        var resepter = new List<Resept>
        {
            CreateTestResept(1, "Brød", 1),
            CreateTestResept(2, "Kake", 1),
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(resepter);

        var handler = new GetAllResepterHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllResepterQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Brød", result[0].Navn);
        Assert.Equal("Kake", result[1].Navn);
    }

    [Fact]
    public async Task GetAllResepterHandler_EmptyList_ReturnsEmpty()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Resept>());

        var handler = new GetAllResepterHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllResepterQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    #endregion

    #region GetReseptByIdHandler

    [Fact]
    public async Task GetReseptByIdHandler_Existing_ReturnsReseptDto()
    {
        var resept = CreateTestResept(5, "Focaccia", 1);
        _repositoryMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(resept);

        var handler = new GetReseptByIdHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetReseptByIdQuery(5), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
        Assert.Equal("Focaccia", result.Navn);
    }

    [Fact]
    public async Task GetReseptByIdHandler_NonExisting_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Resept?)null);

        var handler = new GetReseptByIdHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetReseptByIdQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    #endregion

    #region CreateReseptHandler

    [Fact]
    public async Task CreateReseptHandler_HappyPath_CreatesReseptWithLinjer()
    {
        var ferdigvare = new Artikkel { Navn = "Brød", Enhet = "STK" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ferdigvare, 10);

        var ravare1 = new Artikkel { Navn = "Hvetemel", Enhet = "kg" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ravare1, 20);
        var ravare2 = new Artikkel { Navn = "Gjær", Enhet = "kg" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ravare2, 21);

        var command = new CreateReseptCommand(
            Navn: "Grovt brød",
            FerdigvareId: 10,
            Beskrivelse: "Surdegsbrød med grovmel",
            AntallPortjoner: 10,
            Instruksjoner: "Bland og hev",
            Linjer: new List<CreateLinjeCmd>
            {
                new(RavareId: 20, Mengde: 0.5m, Enhet: "kg", Rekkefolge: 1, Kommentar: null),
                new(RavareId: 21, Mengde: 0.02m, Enhet: "kg", Rekkefolge: 2, Kommentar: "Bruk fersk gjær"),
            });

        Resept? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Resept>(), It.IsAny<CancellationToken>()))
            .Callback<Resept, CancellationToken>((r, _) => { captured = r; typeof(BaseEntity).GetProperty("Id")!.SetValue(r, 99); })
            .Returns(Task.CompletedTask);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(ferdigvare);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(20, It.IsAny<CancellationToken>())).ReturnsAsync(ravare1);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(21, It.IsAny<CancellationToken>())).ReturnsAsync(ravare2);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateReseptHandler(_repositoryMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(99, result);
        Assert.NotNull(captured);
        Assert.Equal("Grovt brød", captured.Navn);
        Assert.Equal(10, captured.FerdigvareId);
        Assert.True(captured.Aktiv);
        Assert.Equal(2, captured.Linjer.Count);
        Assert.Equal(0.5m, captured.Linjer.First(l => l.RavareId == 20).Mengde);
        Assert.Equal("Bruk fersk gjær", captured.Linjer.First(l => l.RavareId == 21).Kommentar);
    }

    [Fact]
    public async Task CreateReseptHandler_FerdigvareNotFound_Throws()
    {
        var command = new CreateReseptCommand(
            Navn: "Test", FerdigvareId: 999, Beskrivelse: null,
            AntallPortjoner: 1, Instruksjoner: null,
            Linjer: new List<CreateLinjeCmd>());

        _artikkelRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artikkel?)null);

        var handler = new CreateReseptHandler(_repositoryMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateReseptHandler_RavareNotFound_Throws()
    {
        var ferdigvare = new Artikkel { Navn = "Brød", Enhet = "STK" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ferdigvare, 10);

        var command = new CreateReseptCommand(
            Navn: "Test", FerdigvareId: 10, Beskrivelse: null,
            AntallPortjoner: 1, Instruksjoner: null,
            Linjer: new List<CreateLinjeCmd>
            {
                new(RavareId: 999, Mengde: 1, Enhet: "kg", Rekkefolge: 1, Kommentar: null),
            });

        _artikkelRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(ferdigvare);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artikkel?)null);

        var handler = new CreateReseptHandler(_repositoryMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateReseptHandler_NoLinjer_CreatesReseptWithEmptyLinjer()
    {
        var ferdigvare = new Artikkel { Navn = "Brød", Enhet = "STK" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ferdigvare, 10);

        var command = new CreateReseptCommand(
            Navn: "Tom resept", FerdigvareId: 10, Beskrivelse: null,
            AntallPortjoner: 1, Instruksjoner: null,
            Linjer: new List<CreateLinjeCmd>());

        Resept? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Resept>(), It.IsAny<CancellationToken>()))
            .Callback<Resept, CancellationToken>((r, _) => captured = r)
            .Returns(Task.CompletedTask);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(ferdigvare);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateReseptHandler(_repositoryMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Empty(captured.Linjer);
    }

    #endregion

    #region UpdateReseptHandler

    [Fact]
    public async Task UpdateReseptHandler_HappyPath_UpdatesFieldsAndLinjer()
    {
        var resept = CreateTestResept(1, "Gammel navn", 1);
        resept.Linjer.Add(new ReseptLinje { RavareId = 20, Mengde = 0.5m, Enhet = "kg", Rekkefolge = 1 });

        var ferdigvare = new Artikkel { Navn = "Brød", Enhet = "STK" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ferdigvare, 10);
        var nyRavare = new Artikkel { Navn = "Rugmel", Enhet = "kg" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(nyRavare, 22);

        var command = new UpdateReseptCommand(
            Id: 1,
            Navn: "Oppdatert navn",
            FerdigvareId: 10,
            Beskrivelse: "Ny beskrivelse",
            AntallPortjoner: 20,
            Instruksjoner: "Nye instrukser",
            Aktiv: true,
            Linjer: new List<UpdateLinjeCmd>
            {
                new(RavareId: 22, Mengde: 1.0m, Enhet: "kg", Rekkefolge: 1, Kommentar: null),
            });

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(resept);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(ferdigvare);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(22, It.IsAny<CancellationToken>())).ReturnsAsync(nyRavare);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateReseptHandler(_repositoryMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.Equal("Oppdatert navn", resept.Navn);
        Assert.Equal("Ny beskrivelse", resept.Beskrivelse);
        Assert.Equal(20, resept.AntallPortjoner);
        Assert.Equal(2, resept.Versjon);
        Assert.Single(resept.Linjer);
        Assert.Equal(22, resept.Linjer.First().RavareId);
        _repositoryMock.Verify(r => r.Update(resept), Times.Once);
    }

    [Fact]
    public async Task UpdateReseptHandler_NonExisting_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Resept?)null);

        var handler = new UpdateReseptHandler(_repositoryMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(
            new UpdateReseptCommand(999, "Navn", 1, null, 1, null, true,
                new List<UpdateLinjeCmd>()),
            CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateReseptHandler_FerdigvareNotFound_Throws()
    {
        var resept = CreateTestResept(1, "Test", 1);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(resept);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artikkel?)null);

        var handler = new UpdateReseptHandler(_repositoryMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                new UpdateReseptCommand(1, "Navn", 999, null, 1, null, true,
                    new List<UpdateLinjeCmd>()),
                CancellationToken.None));
    }

    [Fact]
    public async Task UpdateReseptHandler_DeaktivererResept_SetsAktivFalse()
    {
        var resept = CreateTestResept(1, "Aktiv resept", 1);

        var command = new UpdateReseptCommand(
            Id: 1, Navn: "Aktiv resept", FerdigvareId: 1, Beskrivelse: null,
            AntallPortjoner: 1, Instruksjoner: null, Aktiv: false,
            Linjer: new List<UpdateLinjeCmd>());

        var ferdigvare = new Artikkel { Navn = "Brød", Enhet = "STK" };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(ferdigvare, 1);
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(resept);
        _artikkelRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ferdigvare);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateReseptHandler(_repositoryMock.Object, _artikkelRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.False(resept.Aktiv);
    }

    #endregion

    #region DeleteReseptHandler

    [Fact]
    public async Task DeleteReseptHandler_Existing_DeletesAndReturnsTrue()
    {
        var resept = CreateTestResept(7, "Slett meg", 1);

        _repositoryMock.Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(resept);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteReseptHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new DeleteReseptCommand(7), CancellationToken.None);

        Assert.True(result);
        _repositoryMock.Verify(r => r.Delete(resept), Times.Once);
    }

    [Fact]
    public async Task DeleteReseptHandler_NonExisting_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Resept?)null);

        var handler = new DeleteReseptHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new DeleteReseptCommand(999), CancellationToken.None);

        Assert.False(result);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Resept>()), Times.Never);
    }

    #endregion

    // --- Helpers ---
    private static Resept CreateTestResept(int id, string navn, int ferdigvareId)
    {
        var resept = new Resept
        {
            Navn = navn,
            FerdigvareId = ferdigvareId,
            Beskrivelse = null,
            AntallPortjoner = 1,
            Instruksjoner = null,
            Aktiv = true,
            Versjon = 1,
            OpprettetDato = DateTime.UtcNow,
            SistEndret = DateTime.UtcNow,
            Linjer = new List<ReseptLinje>(),
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(resept, id);
        return resept;
    }
}
