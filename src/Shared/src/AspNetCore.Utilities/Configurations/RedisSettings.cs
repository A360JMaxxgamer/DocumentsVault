namespace AspNetCore.Utilities.Configurations;

public record RedisSettings
{
    public string Host { get; init; } = "localhost:7000";
}