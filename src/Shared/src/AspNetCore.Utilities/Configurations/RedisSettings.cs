using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Utilities.Configurations;

[ExcludeFromCodeCoverage]
public record RedisSettings
{
    public string Host { get; init; } = "localhost:7000";
}