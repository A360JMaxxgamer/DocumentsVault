namespace Documents.Service.Models;

/// <summary>
/// Container for metadata of a document
/// </summary>
public record Metadata
{
    /// <summary>
    /// Title of the document
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A list of custom tags
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Text of the document
    /// </summary>
    public string Text { get; set; } = string.Empty;
}