using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Utilities.Configurations;

[ExcludeFromCodeCoverage]
public record NatsSettings()
{
    public string Endpoint { get; init; } = "nats://localhost:4222";
}