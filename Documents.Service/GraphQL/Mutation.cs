using Documents.Service.GraphQL.Exceptions;
using Documents.Service.Models;
using MongoDB.Driver;

namespace Documents.Service.GraphQL;

public class Mutation
{
    /// <summary>
    /// Adds a new document
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="fileIds">A list of all associated files</param>
    /// <param name="metadata">Metadata of document</param>
    /// <returns>Created document</returns>
    [Error(typeof(FileIdsExistOnDocumentException))]
    public Document AddDocument([Service] IMongoCollection<Document> collection, List<Guid> fileIds, Metadata metadata)
    {
        var existingDocument = collection
            .Find(doc => doc.FileIds.Any(fileIds.Contains))
            .FirstOrDefault();

        if (existingDocument is not null)
        {
            throw new FileIdsExistOnDocumentException(existingDocument.Id);
        }
        
        var creationDate = DateTime.UtcNow;
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Metadata = metadata,
            CreationDate = creationDate,
            ModificationDate = creationDate,
            FileIds = fileIds
        };

        return document;
    }
    
    /// <summary>
    /// Deletes a document by its id
    /// </summary>
    /// <param name="documentId">Id of the document to delete</param>
    /// <exception cref="NotImplementedException"></exception>
    /// <returns>Updated document</returns>
    public Document DeleteDocument(Guid documentId) => throw new NotImplementedException();

    /// <summary>
    /// Adds <paramref name="tags"/> to the document with the <paramref name="documentId"/>
    /// </summary>
    /// <param name="documentId">Id of the document</param>
    /// <param name="tags">Tags to add</param>
    /// <returns>Updated document</returns>
    public Document AddTags(Guid documentId, string[] tags) => throw new NotImplementedException();
    
    /// <summary>
    /// Deletes <paramref name="tags"/> off the document with the <paramref name="documentId"/>
    /// </summary>
    /// <param name="documentId">Id of the document</param>
    /// <param name="tags">Tags to delete</param>
    /// <returns>Updated document</returns>
    public Document DeleteTags(Guid documentId, string[] tags) => throw new NotImplementedException();

    /// <summary>
    /// Updates the title of the document
    /// </summary>
    /// <param name="documentId">Id of the document</param>
    /// <param name="title">New title</param>
    /// <returns>Updated document</returns>
    public Document UpdateDocumentTitle(Guid documentId, string title) => throw new NotImplementedException();

    /// <summary>
    /// Updates the text of the document
    /// </summary>
    /// <param name="documentId">Id of the document</param>
    /// <param name="text">New text</param>
    /// <returns>Updated document</returns>
    public Document UpdateDocumentText(Guid documentId, string text) => throw new NotImplementedException();
}