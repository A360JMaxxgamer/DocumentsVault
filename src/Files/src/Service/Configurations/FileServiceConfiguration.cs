using AspNetCore.Utilities.Configurations;

namespace Files.Service.Configurations;

public record FileServiceConfiguration
{
    public NatsSettings Nats { get; init; } = new();

    public MinioSettings MinioSettings { get; } = new();
    
    public RedisSettings Redis { get; init; } = new();
}

