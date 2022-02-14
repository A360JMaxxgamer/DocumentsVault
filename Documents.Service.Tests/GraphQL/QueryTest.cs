using System;
using Documents.Service.GraphQL;
using Documents.Service.Models;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Documents.Service.Tests.GraphQL;

public class QueryTest
{
    [Fact]
    public void GetDocuments_Should_Return_Executable()
    {
        // Arrange
        var query = new Query();
        var collectionMock = new Mock<IMongoCollection<Document>>();

        // Act
        var result = query.GetDocuments(collectionMock.Object);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetDocumentByIdShould_Return_Executable()
    {
        // Arrange
        var query = new Query();
        var collectionMock = new Mock<IMongoCollection<Document>>();

        // Act
        var result = query.GetDocumentById(collectionMock.Object, Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
    }
}