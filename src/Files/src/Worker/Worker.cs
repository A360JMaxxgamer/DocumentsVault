using System.Net;
using Confluent.Kafka;
using Files.Grpc;
using Files.Worker.Analysis;
using Files.Worker.Configurations;
using Grpc.Net.Client;
using static Files.Grpc.DownloadService;

namespace Files.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IFileAnalyzer _fileAnalyzer;
    private readonly FilesWorkerConfiguration _filesWorkerConfiguration;

    public Worker(
        ILogger<Worker> logger,
        IFileAnalyzer fileAnalyzer,
        FilesWorkerConfiguration filesWorkerConfiguration)
    {
        _logger = logger;
        _fileAnalyzer = fileAnalyzer;
        _filesWorkerConfiguration = filesWorkerConfiguration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = CreateConsumer();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {UtcNow}", DateTimeOffset.UtcNow);
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var fileId = consumeResult.Message.Value;
                await HandleFile(fileId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Message consumption failed");
            }
        }
    }

    private async Task HandleFile(string fileId)
    {
        _logger.LogInformation("Start analysis for {FileId}", fileId);

        using var channel = GrpcChannel.ForAddress(_filesWorkerConfiguration.FileServiceGrpc);
        var client = new DownloadServiceClient(channel);
        var downloadResult = await client.DownloadDocumentAsync(new DocumentDownloadRequest
        {
            FileId = fileId
        });
        var bytes = downloadResult.Data.ToByteArray();
        
        _logger.LogInformation("Download of file finished ({Bytes} bytes received)", bytes.Length);
        await _fileAnalyzer.AnalyzeFileAsync(fileId, bytes);
    }

    private IConsumer<Ignore, string> CreateConsumer()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _filesWorkerConfiguration.BootstrapServers,
            ClientId = Dns.GetHostName()
        };

        var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig)
            .Build();
        return consumer;
    }
}