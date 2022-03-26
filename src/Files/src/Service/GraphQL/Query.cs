using Files.Service.Handlers;
using Files.Service.Models;

namespace Files.Service.GraphQL;

public class Query
{
    public Task<IEnumerable<PreSignedUrl>> GetDocumentDownloadUrl(
        [Service] IFileUrlProvider fileUrlProvider,
        Guid documentId) =>
        fileUrlProvider.GetPreSignedDownloadUrls(documentId);
}