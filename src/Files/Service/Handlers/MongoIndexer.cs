using Files.Service.Models;
using MongoDB.Driver;

namespace Files.Service.Handlers;

public class MongoIndexer : IUploadIndexer
{
    private readonly IMongoCollection<UploadFile> _mongoCollection;

    public MongoIndexer(IMongoCollection<UploadFile> mongoCollection)
    {
        _mongoCollection = mongoCollection;
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
        await _mongoCollection.InsertOneAsync(uploadFile, cancellationToken: cancellationToken);
        return uploadFile;
    }
}