using System.Text;
using Files.Worker.Analysis;
using Files.Worker.Configurations;
using NATS.Client;
using NATS.Client.Rx;

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
        using var natsConnection = new ConnectionFactory()
            .CreateConnection(_filesWorkerConfiguration.Nats.Endpoint);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {UtcNow}", DateTimeOffset.UtcNow);
            var messageStream = natsConnection
                .SubscribeAsync("fileUpload")
                .ToObservable()
                .ToAsyncEnumerable()
                .WithCancellation(stoppingToken);
            await foreach (var fileUpload in messageStream)
            {
                try
                {
                    var uploadedFile = Encoding.UTF8.GetString(fileUpload.Data);
                    await using var dataStream = await new HttpClient().GetStreamAsync(new Uri(uploadedFile), stoppingToken);
                    await HandleFile(dataStream);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Message consumption failed");
                }
            }
        }
    }

    private async Task HandleFile(Stream dataStream)
    {
        var fileId = Guid.NewGuid();
        _logger.LogInformation("Start analysis for {FileId}", fileId);
        await _fileAnalyzer.AnalyzeFileAsync(fileId, dataStream);
    }
}