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
    public void DeleteDocument(Guid documentId) => throw new NotImplementedException();
}