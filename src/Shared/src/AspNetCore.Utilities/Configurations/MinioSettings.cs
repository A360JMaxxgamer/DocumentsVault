using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Utilities.Configurations;

[ExcludeFromCodeCoverage]
public record MinioSettings
{
    public string Endpoint { get; set; } ="localhost:9000";
    public string UploadBucket { get; set; } = "upload";
    
    public string IndexBucket { get; init; } = "index";

    public string Username { get; set; } = "debug";

    public string Password { get; set; } = "debug123";
}