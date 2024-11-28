﻿using Chirp.Core;
using Chirp.Infrastructure.Test.Stub;

namespace Chirp.Test;
using Xunit;
using Chirp.Infrastructure.Cheeps;
using Chirp.Web.Pages.Models;

public class CheepServiceUnitTests
{
    [Fact]
    public void GetCheeps_ReturnsCheeps()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo);
        // Act
        List<CheepDto> cheeps = cheepService.GetCheeps(1, "Author1");

        // Assert
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
    }
    
    [Fact] 
    public void GetCheepsFromAuthor_ReturnsCheeps()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo);
        // Act
        List<CheepDto> cheeps = cheepService.GetCheepsFromAuthor(1, "Author1", "Author1");

        // Assert
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
    }
    
    [Fact]
    public void TimestampToCEST_ConvertsCorrectly()
    {
        // Arrange
        var expectedTime = "18-10-2021 16:38:10";

        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService service = new CheepService(cheepRepo);

        // Act
        var result = service.GetCheeps(1, "Author1");

        // Assert
        Assert.Equal(expectedTime, CheepModel.TimestampToCEST(result[0].Timestamp));
    }
    
    [Fact]
    public void GetAuthorByName_ReturnsAuthor()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo);
        // Act
        Author author = cheepService.GetAuthorByName("Author1");

        // Assert
        Assert.NotNull(author);
        Assert.Equal("Author1", author.UserName);
    }
    
    [Fact]
    public void GetAuthorByEmail_ReturnsAuthor()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo);
        // Act
        Author author = cheepService.GetAuthorByEmail("au1@itu.dk");
        
        // Assert
        Assert.NotNull(author);
        Assert.Equal("Author1", author.UserName);
    }
}