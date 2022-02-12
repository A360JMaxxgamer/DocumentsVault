using System;
using Files.Service.GraphQL;
using Files.Service.Models;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Files.Service.Tests.GraphQL;

public class QueryTest
{
    [Fact]
    public void GetUploadFiles_Should_Return_Executable()
    {
        // Arrange
        var query = new Query();
        var collectionMock = new Mock<IMongoCollection<UploadFile>>();

        // Act
        var result = query.GetUploadFiles(collectionMock.Object);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetUploadFilesByIdShould_Return_Executable()
    {
        // Arrange
        var query = new Query();
        var collectionMock = new Mock<IMongoCollection<UploadFile>>();

        // Act
        var result = query.GetUploadFileById(collectionMock.Object, Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
    }
}