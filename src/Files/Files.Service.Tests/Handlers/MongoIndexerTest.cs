using System;
using System.Threading.Tasks;
using Files.Service.Handlers;
using Files.Service.Models;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Files.Service.Tests.Handlers;

public class MongoIndexerTest
{
    [Fact]
    public async Task Should_InsertAsync_Returns_Result()
    {
        var mongoCollection = new Mock<IMongoCollection<UploadFile>>();
        var mongoIndexer = new MongoIndexer(mongoCollection.Object);
        const string originalFileName = "original";
        const string fileName = "orig";

        // Act
        var result = await mongoIndexer.InsertAsync(Guid.NewGuid(), fileName, originalFileName);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(fileName, result.FileName);
        Assert.Equal(originalFileName, result.OriginalFileName);

        var defaultDateTime = default(DateTime);
        Assert.NotEqual(defaultDateTime, result.UploadDate);
    }
}