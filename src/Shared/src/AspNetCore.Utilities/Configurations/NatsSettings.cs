namespace AspNetCore.Utilities.Configurations;

public record NatsSettings()
{
    public string Endpoint { get; init; } = "nats://localhost:4222";
}