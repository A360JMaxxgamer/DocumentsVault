namespace Files.Service.Models;

public record PreSignedUrl
{
    public string Url { get; init; } = string.Empty;
}