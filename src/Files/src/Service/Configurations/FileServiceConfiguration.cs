namespace Files.Service.Configurations;

public record FileServiceConfiguration
{
    public string NatsEndpoint { get; set; } = "http://localhost:4222";

    public MinioSettings MinioSettings { get; set; } = new();
}

public record MinioSettings
{
    public string Endpoint { get; set; } ="localhost:9000";
    public string UploadBucket { get; set; } = "upload";

    public string Username { get; set; } = "debug";

    public string Password { get; set; } = "debug123";
}