using Files.Service.Models;
using HotChocolate.Data;
using MongoDB.Driver;

namespace Files.Service.GraphQL;

public class Query
{
    /// <summary>
    /// Get a upload file by its id
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [UseFirstOrDefault]
    public IExecutable<UploadFile> GetUploadFileById(
        [Service] IMongoCollection<UploadFile> collection,
        Guid id) =>
        collection
            .Find(x => x.Id == id)
            .AsExecutable();

    /// <summary>
    /// Get a list of upload files based on paging, filter and sorting options
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    [UsePaging]
    [UseProjection]
    [UseSorting]
    [UseFiltering]
    public IExecutable<UploadFile> GetUploadFiles([Service] IMongoCollection<UploadFile> collection) =>
        collection.AsExecutable();
}