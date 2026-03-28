using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Articles.Commands.CreateArticle;
using LagerPro.Application.Features.Articles.Commands.DeleteArticle;
using LagerPro.Application.Features.Articles.Commands.UpdateArticle;
using LagerPro.Application.Features.Articles.Queries.GetAllArticles;
using LagerPro.Application.Features.Articles.Queries.GetArticleById;
using LagerPro.Contracts.Dtos.Articles;
using LagerPro.Domain.Common;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;
using Moq;

namespace LagerPro.Application.Tests;

public class ArticleTests
{
    private readonly Mock<IArtikkelRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public ArticleTests()
    {
        _repositoryMock = new Mock<IArtikkelRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    #region CreateArticleHandler

    [Fact]
    public async Task CreateArticleHandler_HappyPath_SetsFieldsAndSaves()
    {
        // Arrange
        var command = new CreateArticleCommand(
            ArtikkelNr: "RAV-001",
            Navn: "Hvetemel",
            Enhet: "kg",
            Type: "Ravare",
            Beskrivelse: "Brenner Hvetemel",
            Strekkode: "123456789",
            Kategori: "Mel",
            Innpris: 12.50m,
            Utpris: 18.00m,
            MinBeholdning: 100
        );

        Artikkel? capturedArticle = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Artikkel>(), It.IsAny<CancellationToken>()))
            .Callback<Artikkel, CancellationToken>((a, _) => capturedArticle = a)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateArticleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedArticle);
        Assert.Equal("RAV-001", capturedArticle!.ArtikkelNr);
        Assert.Equal("Hvetemel", capturedArticle.Navn);
        Assert.Equal(ArtikelType.Ravare, capturedArticle.Type);
        Assert.Equal("kg", capturedArticle.Enhet);
        Assert.True(capturedArticle.Aktiv);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Artikkel>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("Ravare", ArtikelType.Ravare)]
    [InlineData("Ferdigvare", ArtikelType.Ferdigvare)]
    [InlineData("Emballasje", ArtikelType.Emballasje)]
    [InlineData("RAVARE", ArtikelType.Ravare)]
    [InlineData("ferdigVare", ArtikelType.Ferdigvare)]
    public async Task CreateArticleHandler_ParsesType_CaseInsensitive(string typeString, ArtikelType expectedType)
    {
        // Arrange
        var command = new CreateArticleCommand(
            ArtikkelNr: "TEST-001", Navn: "Test", Enhet: "stk",
            Type: typeString, null, null, null, 0, 0, 0);

        Artikkel? capturedArticle = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Artikkel>(), It.IsAny<CancellationToken>()))
            .Callback<Artikkel, CancellationToken>((a, _) => capturedArticle = a)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateArticleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedArticle);
        Assert.Equal(expectedType, capturedArticle!.Type);
    }

    [Fact]
    public async Task CreateArticleHandler_InvalidType_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateArticleCommand(
            ArtikkelNr: "TEST-001", Navn: "Test", Enhet: "stk",
            Type: "UgyldigType", null, null, null, 0, 0, 0);

        var handler = new CreateArticleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    #endregion

    #region GetAllArticlesHandler

    [Fact]
    public async Task GetAllArticlesHandler_ReturnsAllArticles()
    {
        // Arrange
        var articles = new List<Artikkel>
        {
            CreateTestArticle(1, "RAV-001", "Hvetemel"),
            CreateTestArticle(2, "RAV-002", "Sukker")
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(articles);

        var handler = new GetAllArticlesHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Hvetemel", result[0].Navn);
        Assert.Equal("Sukker", result[1].Navn);
    }

    [Fact]
    public async Task GetAllArticlesHandler_EmptyList_ReturnsEmpty()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Artikkel>());

        var handler = new GetAllArticlesHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetArticleByIdHandler

    [Fact]
    public async Task GetArticleByIdHandler_ExistingArticle_ReturnsArticleDto()
    {
        // Arrange
        var article = CreateTestArticle(42, "RAV-001", "Hvetemel");

        _repositoryMock.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(article);

        var handler = new GetArticleByIdHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetArticleByIdQuery(42), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result!.Id);
        Assert.Equal("RAV-001", result.ArtikkelNr);
        Assert.Equal("Hvetemel", result.Navn);
    }

    [Fact]
    public async Task GetArticleByIdHandler_NonExisting_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artikkel?)null);

        var handler = new GetArticleByIdHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetArticleByIdQuery(999), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region UpdateArticleHandler

    [Fact]
    public async Task UpdateArticleHandler_ExistingArticle_UpdatesAndReturnsTrue()
    {
        // Arrange
        var article = CreateTestArticle(1, "RAV-001", "Hvetemel");

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(article);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateArticleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var command = new UpdateArticleCommand(
            Id: 1,
            ArtikkelNr: "RAV-001-UPD",
            Navn: "Hvetemel oppdatert",
            Enhet: "kg",
            Type: "Ravare",
            Beskrivelse: "Oppdatert",
            Strekkode: "999",
            Kategori: "Mel",
            Innpris: 15.00m,
            Utpris: 22.00m,
            MinBeholdning: 50,
            Aktiv: true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal("RAV-001-UPD", article.ArtikkelNr);
        Assert.Equal("Hvetemel oppdatert", article.Navn);
        Assert.Equal(15.00m, article.Innpris);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateArticleHandler_NonExisting_ReturnsFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artikkel?)null);

        var handler = new UpdateArticleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        var command = new UpdateArticleCommand(
            Id: 999, ArtikkelNr: "X", Navn: "X", Enhet: "x",
            Type: "Ravare", null, null, null, 0, 0, 0, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeleteArticleHandler

    [Fact]
    public async Task DeleteArticleHandler_ExistingArticle_DeletesAndReturnsTrue()
    {
        // Arrange
        var article = CreateTestArticle(1, "RAV-001", "Hvetemel");

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(article);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteArticleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(new DeleteArticleCommand(1), CancellationToken.None);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(r => r.Delete(article), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteArticleHandler_NonExisting_ReturnsFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Artikkel?)null);

        var handler = new DeleteArticleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(new DeleteArticleCommand(999), CancellationToken.None);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Artikkel>()), Times.Never);
    }

    #endregion

    // --- Helper ---
    private static Artikkel CreateTestArticle(int id, string artikkelNr, string navn)
    {
        // Use reflection since Id has protected setter
        var article = new Artikkel
        {
            ArtikkelNr = artikkelNr,
            Navn = navn,
            Enhet = "kg",
            Type = ArtikelType.Ravare,
            Aktiv = true
        };
        typeof(BaseEntity).GetProperty("Id")!.SetValue(article, id);
        return article;
    }
}
