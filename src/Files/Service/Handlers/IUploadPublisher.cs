using Files.Service.Models;

namespace Files.Service.Handlers;

public interface IUploadPublisher
{
    Task PublishUploadAsync(UploadFile uploadedFile, CancellationToken cancellationToken = default);
}