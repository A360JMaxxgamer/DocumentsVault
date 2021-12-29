using System.Runtime.CompilerServices;
using Files.Grpc;

[assembly: InternalsVisibleTo("Files.Service.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Files.Service.Handlers
{
    internal interface IDocumentUploadHandler
    {
        /// <summary>
        /// Uploads a all files of a document and triggers further handling.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DocumentUploadResult> UploadDocumentAsync(DocumentUpload request,
            CancellationToken cancellationToken = default);
    }
}