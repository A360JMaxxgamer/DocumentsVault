using System.Runtime.CompilerServices;
using Files.Grpc;
using Files.Service.Handlers;
using Grpc.Core;

[assembly: InternalsVisibleTo("Files.Service.Tests")]

namespace Files.Service.Services
{
    internal class UploadService : Grpc.UploadService.UploadServiceBase
    {
        private readonly IDocumentUploadHandler _documentUploadHandler;
        private readonly IUploadIndexer _uploadIndexer;
        private readonly IUploadPublisher _uploadPublisher;

        public UploadService(
            IDocumentUploadHandler documentUploadHandler, 
            IUploadIndexer uploadIndexer,
            IUploadPublisher uploadPublisher)
        {
            _documentUploadHandler = documentUploadHandler;
            _uploadIndexer = uploadIndexer;
            _uploadPublisher = uploadPublisher;
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
            
            foreach (var fileUploadResult in result.Results.SelectMany(r => r.UploadedFilesResult).ToList())
            {
                var fileId = Guid.Parse(fileUploadResult.FileId);
                var fileName = fileUploadResult.FileName;
                var uploadedFile = await _uploadIndexer.InsertAsync(fileId, fileName, fileName, context.CancellationToken);
                await _uploadPublisher.PublishUploadAsync(uploadedFile, context.CancellationToken);
            }

            return result;
        }
    }
}