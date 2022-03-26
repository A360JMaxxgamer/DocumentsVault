using Files.Service.Models;

namespace Files.Service.Handlers;

public interface IFilePublisher
{
    Task PublishUploadAsync(CancellationToken cancellationToken = default);
}