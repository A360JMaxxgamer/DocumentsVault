using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Files.Grpc;
using Files.Service.Handlers;
using Grpc.Core;

[assembly:InternalsVisibleTo("Files.Service.Tests")]
namespace Files.Service.Services
{
    internal class UploadService : Grpc.UploadService.UploadServiceBase
    {
        private readonly IDocumentUploadHandler _documentUploadHandler;

        public UploadService(IDocumentUploadHandler documentUploadHandler)
        {
            _documentUploadHandler = documentUploadHandler;
        }
        
        /// <inheritdoc />
        public override async Task<UploadDocumentsResult> UploadDocuments(
            IAsyncStreamReader<DocumentUpload> requestStream,
            ServerCallContext context)
        {
            UploadDocumentsResult result = new();

            while (await requestStream.MoveNext(context.CancellationToken))
            {
                var documentUpload = requestStream.Current;

                var documentUploadResult =
                    await _documentUploadHandler.UploadDocumentAsync(documentUpload, context.CancellationToken);
                
                result.Results.Add(documentUploadResult);
            }

            return result;
        }
    }
}