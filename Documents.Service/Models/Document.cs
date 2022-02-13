using MongoDB.Bson.Serialization.Attributes;

namespace Documents.Service.Models;

/// <summary>
/// Document which contains various information e.g. a letter or
/// </summary>
public record Document
{
    /// <summary>
    /// Id of the document
    /// </summary>
    [BsonId]
    public Guid Id { get; set; }

    /// <summary>
    /// Date the document was initially created
    /// </summary>
    public DateTime CreationDate { get; set; }
    
    /// <summary>
    /// Date the document was modified last 
    /// </summary>
    public DateTime ModificationDate { get; set; }

    /// <summary>
    /// Additional metadata of the document
    /// </summary>
    public Metadata Metadata { get; set; } = new();

    /// <summary>
    /// Ids of the files
    /// </summary>
    public List<Guid> FileIds { get; set; } = new();
}