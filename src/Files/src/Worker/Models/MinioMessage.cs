using System.Text.Json.Serialization;

namespace Files.Worker.Models;

public record MinioMessage
{
    [JsonPropertyName("Records")]
    public List<MinioRecord> Records { get; set; } = new();
}

public record MinioRecord
{
    [JsonPropertyName("s3")]
    public MinioS3 Container { get; set; } = new();
}

public record MinioS3
{
    public MinioBucket Bucket { get; set; } = new();

    public MinioUploadObject Object { get; set; } = new();
}

public record MinioUploadObject
{
    public string Key { get; set; } = string.Empty;
    
    public string ContentType { get; set; } = string.Empty;
}

public record MinioBucket
{
    public string Name { get; set; } = string.Empty;
}