using System.Diagnostics.CodeAnalysis;
using System.Net;
using Confluent.Kafka;
using Files.Service.Configurations;
using Files.Service.Models;

namespace Files.Service.Handlers;

[ExcludeFromCodeCoverage]
internal class MessageQueuePublisher : IUploadPublisher
{
    private readonly FileServiceConfiguration _fileServiceConfiguration;

    public MessageQueuePublisher(FileServiceConfiguration fileServiceConfiguration)
    {
        _fileServiceConfiguration = fileServiceConfiguration;
    }

    /// <inheritdoc />
    public Task PublishUploadAsync(UploadFile uploadedFile, CancellationToken cancellationToken = default)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _fileServiceConfiguration.BootstrapServers,
            ClientId = Dns.GetHostName()
        };

        using var producer = new ProducerBuilder<Ignore, string>(producerConfig)
            .Build();

        producer.Produce("fileUploaded", new Message<Ignore, string>
        {
            Value = uploadedFile.Id.ToString()
        });
        return Task.CompletedTask;
    }
}