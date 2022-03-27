using System.Diagnostics.CodeAnalysis;
using AspNetCore.Utilities.Configurations;

namespace Documents.Service.Configurations;

[ExcludeFromCodeCoverage]
public record DocumentServiceConfiguration
{
    public string MongoConnectionString { get; init; } = string.Empty;
    
    public RedisSettings Redis { get; init; } = new();
}