using Documents.Service.Models;
using HotChocolate.Data;
using MongoDB.Driver;

namespace Documents.Service.GraphQL;

public class Query
{
    /// <summary>
    /// Get a document by its id
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [UseFirstOrDefault]
    public IExecutable<Document> GetDocumentById(
        [Service] IMongoCollection<Document> collection,
        Guid id) =>
        collection
            .Find(x => x.Id == id)
            .AsExecutable();

    /// <summary>
    /// Get a list of documents based on paging, filter and sorting options
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    [UsePaging]
    [UseProjection]
    [UseSorting]
    [UseFiltering]
    public IExecutable<Document> GetDocuments([Service] IMongoCollection<Document> collection) =>
        collection.AsExecutable();
}