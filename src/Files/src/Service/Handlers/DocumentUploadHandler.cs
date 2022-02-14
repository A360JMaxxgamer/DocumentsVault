using Files.Grpc;
using Files.Service.Configurations;
using Path = System.IO.Path;

namespace Files.Service.Handlers
{
    internal class DocumentUploadHandler : IDocumentUploadHandler
    {
        private readonly FileServiceConfiguration _fileServiceConfiguration;

        public DocumentUploadHandler(FileServiceConfiguration fileServiceConfiguration)
        {
            _fileServiceConfiguration = fileServiceConfiguration;
        }

        /// <inheritdoc />
        public async Task<DocumentUploadResult> UploadDocumentAsync(
            DocumentUpload request,
            CancellationToken cancellationToken = default)
        {
            var fileTasks = request.Files
                .Select(file => UploadFileAsync(file, cancellationToken))
                .ToList();
            var fileUploadResults = await Task.WhenAll(fileTasks);

            var documentUploadResult = new DocumentUploadResult();
            documentUploadResult.UploadedFilesResult.AddRange(fileUploadResults);
            return documentUploadResult;
        }

        private void EnsureUploadFolderExists()
        {
            if (!Directory.Exists(_fileServiceConfiguration.Folder))
                Directory.CreateDirectory(_fileServiceConfiguration.Folder);
        }

        private async Task<FileUploadResult> UploadFileAsync(FileUpload fileUpload, CancellationToken cancellationToken)
        {
            EnsureUploadFolderExists();

            var fileId = Guid.NewGuid();
            var fileName = Path.Combine(_fileServiceConfiguration.Folder, $"{fileId.ToString()}.{fileUpload.Filetype}");
            await File.WriteAllBytesAsync(fileName, fileUpload.Data.ToByteArray(), cancellationToken);

            return new FileUploadResult
            {
                FileId = fileId.ToString(),
                FileName = fileName
            };
        }
    }
}