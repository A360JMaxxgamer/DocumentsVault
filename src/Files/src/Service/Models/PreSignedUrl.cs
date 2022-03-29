namespace Files.Service.Models;

public record PreSignedUrl
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}