using System.Net;
using Confluent.Kafka;
using Files.Service.Configurations;
using Files.Service.Models;

namespace Files.Service.Handlers;

internal class MessageQueuePublisher : IUploadPublisher
{
    private readonly FileServiceConfiguration _fileServiceConfiguration;

    public MessageQueuePublisher(FileServiceConfiguration fileServiceConfiguration)
    {
        _fileServiceConfiguration = fileServiceConfiguration;
    }
    
    /// <inheritdoc />
    public async Task PublishUploadAsync(UploadFile uploadedFile, CancellationToken cancellationToken = default)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _fileServiceConfiguration.BootstrapServers,
            ClientId = Dns.GetHostName()
        };

        using var producer = new ProducerBuilder<Null, UploadFile>(producerConfig).Build();

        await producer.ProduceAsync("fileUploaded", new Message<Null, UploadFile>
        {
            Value = uploadedFile
        }, cancellationToken);
    }
}