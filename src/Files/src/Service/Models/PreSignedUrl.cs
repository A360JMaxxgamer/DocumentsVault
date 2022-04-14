using System.Diagnostics.CodeAnalysis;

namespace Files.Service.Models;

[ExcludeFromCodeCoverage(Justification = "Container class")]
public record PreSignedUrl
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}