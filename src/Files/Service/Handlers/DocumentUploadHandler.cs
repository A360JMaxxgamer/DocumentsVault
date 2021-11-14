using System;
using System.Threading;
using System.Threading.Tasks;
using Files.Grpc;

namespace Files.Service.Handlers
{
    internal class DocumentUploadHandler : IDocumentUploadHandler
    {
        /// <inheritdoc />
        public Task<DocumentUploadResult> UploadDocumentAsync(
            DocumentUpload request,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}