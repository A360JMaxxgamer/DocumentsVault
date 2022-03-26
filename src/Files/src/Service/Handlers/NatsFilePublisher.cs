using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Text;
using Files.Service.Configurations;
using Minio;
using NATS.Client;

namespace Files.Service.Handlers;

[ExcludeFromCodeCoverage]
internal class NatsFilePublisher : IFilePublisher
{
    private readonly FileServiceConfiguration _fileServiceConfiguration;
    private readonly ILogger<NatsFilePublisher> _logger;

    public NatsFilePublisher(FileServiceConfiguration fileServiceConfiguration, ILogger<NatsFilePublisher> logger)
    {
        _fileServiceConfiguration = fileServiceConfiguration;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishUploadAsync(CancellationToken cancellationToken = default)
    {
        var minioClient = new MinioClient()
            .WithCredentials(
                _fileServiceConfiguration.MinioSettings.Username,
                _fileServiceConfiguration.MinioSettings.Password)
            .WithEndpoint(_fileServiceConfiguration.MinioSettings.Endpoint)
            .Build();

        var listArgs = new ListObjectsArgs()
            .WithBucket(_fileServiceConfiguration.MinioSettings.UploadBucket);
        
        var newDocuments = await minioClient
            .ListObjectsAsync(listArgs)
            .Where(item => !item.IsDir)
            .ToList();
        _logger.LogInformation("Found {Count} new documents", newDocuments.Count);
        
        var natsConnection = new ConnectionFactory()
            .CreateConnection(_fileServiceConfiguration.NatsEndpoint);

        foreach (var newDocument in newDocuments)
        {
            var getPreSignerArgs = new PresignedGetObjectArgs()
                .WithObject(newDocument.Key)
                .WithExpiry((int) TimeSpan.FromDays(7).TotalSeconds)
                .WithBucket(_fileServiceConfiguration.MinioSettings.UploadBucket);

            var preSignedGetUrl = await minioClient.PresignedGetObjectAsync(getPreSignerArgs);
            
            _logger.LogInformation("Inserting {Key} into queue", newDocument.Key);
            natsConnection.Publish("fileUpload", Encoding.UTF8.GetBytes(preSignedGetUrl));
        }
    }
}