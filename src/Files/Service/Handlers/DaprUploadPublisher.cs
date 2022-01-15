using Dapr.Client;
using Files.Service.Models;

namespace Files.Service.Handlers;

internal class DaprUploadPublisher : IUploadPublisher
{
    private readonly DaprClient _daprClient;

    public DaprUploadPublisher(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    /// <inheritdoc />
    public async Task PublishUploadAsync(UploadFile uploadedFile, CancellationToken cancellationToken = default)
    {
        await _daprClient.PublishEventAsync("documents-vault", "uploads", uploadedFile, cancellationToken);
    }
}