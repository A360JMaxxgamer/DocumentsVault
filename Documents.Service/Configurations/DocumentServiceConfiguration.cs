using System.Diagnostics.CodeAnalysis;

namespace Documents.Service.Configurations;

[ExcludeFromCodeCoverage]
public record DocumentServiceConfiguration
{
    public string MongoConnectionString { get; set; } = string.Empty;
}