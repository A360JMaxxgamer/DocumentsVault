using Files.Service.Models;

namespace Files.Service.Handlers;

public interface IUploadIndexer
{
    Task<UploadFile> InsertAsync(Guid id, string fileName, string originalFileName,
        CancellationToken cancellationToken = default);

    Task<UploadFile> GetAsync(Guid fileId);
}