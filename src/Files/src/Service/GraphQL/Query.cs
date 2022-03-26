using Files.Service.Handlers;
using Files.Service.Models;

namespace Files.Service.GraphQL;

public class Query
{
    public Task<PreSignedUrl> GetDocumentDownloadUrl(
        [Service] IFileUrlProvider fileUrlProvider,
        Guid documentId) =>
        fileUrlProvider.GetPreSignedDownloadUrl(documentId);
}