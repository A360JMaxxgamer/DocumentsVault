using Documents.Service.Models;

namespace Documents.Service.GraphQL;

public class Mutation
{
    /// <summary>
    /// Create a new document
    /// </summary>
    /// <param name="fileIds">A list of all associated files</param>
    /// <param name="metadata">Metadata of document</param>
    /// <returns>Created document</returns>
    public Document AddDocument(List<Guid> fileIds, Metadata metadata) => throw new NotImplementedException();
    
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