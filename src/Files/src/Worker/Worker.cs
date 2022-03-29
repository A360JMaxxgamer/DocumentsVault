using System.Text;
using System.Text.Json;
using Files.Worker.Analysis;
using Files.Worker.Configurations;
using Files.Worker.Models;
using NATS.Client;
using NATS.Client.Rx;

namespace Files.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDocumentAnalyzer _documentAnalyzer;
    private readonly FilesWorkerConfiguration _filesWorkerConfiguration;

    public Worker(
        ILogger<Worker> logger,
        IDocumentAnalyzer documentAnalyzer,
        FilesWorkerConfiguration filesWorkerConfiguration)
    {
        _logger = logger;
        _documentAnalyzer = documentAnalyzer;
        _filesWorkerConfiguration = filesWorkerConfiguration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var natsConnection = new ConnectionFactory()
            .CreateConnection(_filesWorkerConfiguration.Nats.Endpoint);

        var stream = natsConnection
            .SubscribeAsync("fileUpload", (_, _) => { })
            .ToObservable()
            .ToAsyncEnumerable()
            .WithCancellation(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await foreach(var message in stream)
            {
                try
                {
                    var encodedString = Encoding.UTF8.GetString(message.Data);
                    var uploadMessage = JsonSerializer.Deserialize<MinioMessage>(encodedString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    if (uploadMessage is null)
                    {
                        continue;
                    }
                    
                    await _documentAnalyzer
                        .AnalyzeDocumentAsync(uploadMessage, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Message consumption failed");
                }

            }
            _logger.LogInformation("Worker running at: {UtcNow}", DateTimeOffset.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
        }
    }
}