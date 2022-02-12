using System;
using System.Threading.Tasks;
using Files.Service.Handlers;
using Files.Service.Models;
using MongoDB.Driver;
using Moq;
using UnitTestHelper.Extensions;
using Xunit;

namespace Files.Service.Tests.Handlers;

public class MongoIndexerTest
{
    [Fact]
    public async Task Should_InsertAsync_Returns_Result()
    {
        var mongoClientMock = new Mock<IMongoClient>();
        mongoClientMock
            .SetupReturnMock(client => client.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
            .SetupReturnMock(db =>
                db.GetCollection<UploadFile>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()));
            
        var mongoIndexer = new MongoIndexer(mongoClientMock.Object);
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