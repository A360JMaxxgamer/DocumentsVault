using System;
using System.Collections.Generic;
using System.Threading;
using Documents.Service.GraphQL;
using Documents.Service.GraphQL.Exceptions;
using Documents.Service.Models;
using MongoDB.Driver;
using Moq;
using UnitTestHelper.Extensions;
using Xunit;

namespace Documents.Service.Tests.GraphQL;

public class MutationTest
{
    [Fact]
    public void AddDocument_Should_Return_Added_Document()
    {
        // Arrange
        var mutation = new Mutation();
        var fileIds = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };
        var metadata = new Metadata
        {
            Tags = new List<string>
            {
                "Unittest",
                "CodeCoverage"
            },
            Text = "...",
            Title = "Title"
        };
        var collectionMock = new Mock<IMongoCollection<Document>>();
        collectionMock
            .SetupReturnMock(collection => collection
                .FindSync(
                    It.IsAny<FilterDefinition<Document>>(), 
                    It.IsAny<FindOptions<Document>>(), 
                    It.IsAny<CancellationToken>()))
            .SetupReturn(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()), false);
        
        // Act 
        var document = mutation.AddDocument(collectionMock.Object, fileIds, metadata);

        // Assert
        Assert.NotEqual(Guid.Empty, document.Id);
        Assert.Equal(document.CreationDate, document.ModificationDate);
        
        Assert.Equal(fileIds[0], document.FileIds[0]);
        Assert.Equal(fileIds[1], document.FileIds[1]);
        
        Assert.Equal(metadata.Title, document.Metadata.Title);
        Assert.Equal(metadata.Text, document.Metadata.Text);
        Assert.NotEmpty(metadata.Tags);
    }
    
    [Fact]
    public void AddDocument_Should_Throw_FileIdsExistOnDocumentException_On_Duplication()
    {
        // Arrange
        var mutation = new Mutation();
        var fileIds = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };
        var metadata = new Metadata
        {
            Tags = new List<string>
            {
                "Unittest",
                "CodeCoverage"
            },
            Text = "...",
            Title = "Title"
        };
        var collectionMock = new Mock<IMongoCollection<Document>>();
        collectionMock
            .SetupReturnMock(collection => collection
                .FindSync(
                    It.IsAny<FilterDefinition<Document>>(),
                    It.IsAny<FindOptions<Document>>(),
                    It.IsAny<CancellationToken>()))
            .SetupReturn(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()), true)
            .SetupReturn(cursor => cursor.Current, new Document[]
            {
                new()
                {
                    Id = Guid.NewGuid()
                }
            });
        
        // Act 
        var exception = Record.Exception(() => mutation.AddDocument(collectionMock.Object, fileIds, metadata));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FileIdsExistOnDocumentException>(exception);
    }
}