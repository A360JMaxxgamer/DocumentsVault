namespace Files.Service.Configurations;

public record FileServiceConfiguration
{
    public string Folder { get; set; } = "./Upload";

    public string BootstrapServers { get; set; } = string.Empty;

    public string MongoConnectionString { get; set; } = string.Empty;
}