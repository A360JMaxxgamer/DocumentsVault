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
    
    [Fact]
    public void DeleteDocument_Should_Throw_DocumentNotFoundException_On_Document_Not_Found()
    {
        // Arrange
        var mutation = new Mutation();
        var collectionMock = new Mock<IMongoCollection<Document>>();
        collectionMock
            .SetupReturnMock(collection => collection
                .FindSync(
                    It.IsAny<FilterDefinition<Document>>(),
                    It.IsAny<FindOptions<Document>>(),
                    It.IsAny<CancellationToken>()))
            .SetupReturn(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()), false);
        
        // Act 
        var exception = Record.Exception(() => mutation.DeleteDocument(collectionMock.Object, Guid.NewGuid()));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<DocumentNotFoundException>(exception);
    }
    
    [Fact]
    public void DeleteDocument_Should_Execute_Without_Errors()
    {
        // Arrange
        var mutation = new Mutation();
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
        var exception = Record.Exception(() => mutation.DeleteDocument(collectionMock.Object, Guid.NewGuid()));

        // Assert
        Assert.Null(exception);
    }
    
    [Fact]
    public void AddTags_Should_Update_Tags_Without_Duplicates()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var mutation = new Mutation();
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
                    Id = docId,
                    Metadata = new Metadata
                    {
                        Tags = new List<string>
                        {
                            "TagOne",
                            "TagThree"
                        }
                    }
                }
            });
        
        // Act 
        var document = mutation.AddTags(collectionMock.Object, docId, new[]
        {
            "TagOne",
            "TagTwo",
        });

        // Assert
        Assert.Equal(3, document.Metadata.Tags.Count);
    }
    
    [Fact]
    public void AddTags_Should_Throw_DocumentNotFoundException_On_Document_Not_Found()
    {
        // Arrange
        var mutation = new Mutation();
        var collectionMock = new Mock<IMongoCollection<Document>>();
        collectionMock
            .SetupReturnMock(collection => collection
                .FindSync(
                    It.IsAny<FilterDefinition<Document>>(),
                    It.IsAny<FindOptions<Document>>(),
                    It.IsAny<CancellationToken>()))
            .SetupReturn(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()), true);
        
        // Act 
        var exception = Record.Exception(() => mutation.AddTags(collectionMock.Object, Guid.NewGuid(), Array.Empty<string>()));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<DocumentNotFoundException>(exception);
    }

    [Fact]
    public void DeleteTags_Should_Remove_Tags()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var mutation = new Mutation();
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
                    Id = docId,
                    Metadata = new Metadata
                    {
                        Tags = new List<string>
                        {
                            "TagOne",
                            "TagTwo",
                            "TagThree"
                        }
                    }
                }
            });
        
        // Act 
        var document = mutation.DeleteTags(collectionMock.Object, docId, new[]
        {
            "TagOne",
            "TagTwo"
        });

        // Assert
        Assert.Single(document.Metadata.Tags);
    }
    
    [Fact]
    public void UpdateDocumentTitle_Should_Update_Title()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var mutation = new Mutation();
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
                    Id = docId,
                    Metadata = new Metadata
                    {
                        Title = "Old"
                    }
                }
            });
        const string title = "NewTitle";
        
        // Act 
        var document = mutation.UpdateDocumentTitle(collectionMock.Object, docId, title);

        // Assert
        Assert.Equal(title, document.Metadata.Title);
    }
    
    [Fact]
    public void UpdateDocumentText_Should_Update_Text()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var mutation = new Mutation();
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
                    Id = docId,
                    Metadata = new Metadata
                    {
                        Text = "Old"
                    }
                }
            });
        const string text = "NewTitle";
        
        // Act 
        var document = mutation.UpdateDocumentText(collectionMock.Object, docId, text);

        // Assert
        Assert.Equal(text, document.Metadata.Text);
    }
}