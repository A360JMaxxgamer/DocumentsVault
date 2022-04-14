using System.Diagnostics.CodeAnalysis;
using AspNetCore.Utilities.Configurations;

namespace Files.Service.Configurations;

[ExcludeFromCodeCoverage(Justification = "Container class only used for deserialization")]
public record FileServiceConfiguration
{
    public NatsSettings Nats { get; init; } = new();

    public MinioSettings MinioSettings { get; } = new();
    
    public RedisSettings Redis { get; init; } = new();
}

