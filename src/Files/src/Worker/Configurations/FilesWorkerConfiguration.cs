namespace Files.Worker.Configurations;

public record FilesWorkerConfiguration
{
    public string BootstrapServers { get; set; } = string.Empty;

    public string FileServiceGrpc { get; set; } = string.Empty;
}