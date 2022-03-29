namespace Messages.Uploads;

public record UploadedDocumentMessage
{
    public string? Bucket { get; init; } = "upload";
    
    public string? Key { get; init; }
}