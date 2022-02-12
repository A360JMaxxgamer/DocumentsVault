using Files.Service.Models;
using Files.Service.Shared;
using MongoDB.Driver;

namespace Files.Service.Handlers;

public class MongoIndexer : IUploadIndexer
{
    private readonly IMongoClient _mongoClient;

    public MongoIndexer(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
    }
    
    /// <inheritdoc />
    public async Task<UploadFile> InsertAsync(
        Guid id, 
        string fileName, 
        string originalFileName,
        CancellationToken cancellationToken = default)
    {
        var uploadFile = new UploadFile
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            OriginalFileName = originalFileName,
            UploadDate = DateTime.UtcNow
        };
        var fileIndexDb = _mongoClient.GetDatabase(ServiceConstants.FileIndexDb);

        var mongoCollection = fileIndexDb.GetCollection<UploadFile>(ServiceConstants.FileCollection);
        await mongoCollection.InsertOneAsync(uploadFile, cancellationToken: cancellationToken);
        return uploadFile;
    }
}