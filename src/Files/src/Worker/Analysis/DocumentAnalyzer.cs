using System.Text;
using Files.Worker.Configurations;
using Files.Worker.Models;
using Minio;
using StrawberryShake;

namespace Files.Worker.Analysis;

internal class DocumentAnalyzer : IDocumentAnalyzer
{
    private readonly MinioClient _minioClient;
    private readonly IApiClient _apiClient;
    private readonly IEnumerable<IDocumentReader> _documentReaders;
    private readonly FilesWorkerConfiguration _filesWorkerConfiguration;
    private readonly ILogger<DocumentAnalyzer> _logger;

    public DocumentAnalyzer(
        IMinioClient minioClient,
        IApiClient apiClient,
        IEnumerable<IDocumentReader> documentReaders,
        FilesWorkerConfiguration filesWorkerConfiguration,
        ILogger<DocumentAnalyzer> logger)
    {
        _minioClient = minioClient.Build();
        _apiClient = apiClient;
        _documentReaders = documentReaders;
        _filesWorkerConfiguration = filesWorkerConfiguration;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task AnalyzeDocumentAsync(MinioMessage uploadedDocumentMessage, CancellationToken token)
    {
        var text = await ReadDocumentTextAsync(uploadedDocumentMessage, token);
        await SaveDocumentAsync(text, token);
        await CleanupUploadFolderAsync(uploadedDocumentMessage, token);
    }

    private async Task CleanupUploadFolderAsync(
        MinioMessage uploadedDocumentMessage,
        CancellationToken token)
    {
        if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs()
                .WithBucket(_filesWorkerConfiguration.Minio.IndexBucket), token))
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs()
                .WithBucket(_filesWorkerConfiguration.Minio.IndexBucket), token);
        }

        foreach (var minioRecord in uploadedDocumentMessage.Records)
        {
            _logger.LogInformation("Copying file {File} to index bucket", minioRecord.Container.Object.Key);
            await _minioClient.CopyObjectAsync(new CopyObjectArgs()
                .WithCopyObjectSource(new CopySourceObjectArgs()
                    .WithBucket(minioRecord.Container.Bucket.Name)
                    .WithObject(minioRecord.Container.Object.Key))
                .WithBucket(_filesWorkerConfiguration.Minio.IndexBucket), token);

            _logger.LogInformation("Removing file {File}",  minioRecord.Container.Object.Key);
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(minioRecord.Container.Bucket.Name)
                .WithObject(minioRecord.Container.Object.Key)
                .WithBypassGovernanceMode(true), token);
        }
    }

    private async Task SaveDocumentAsync(string text, CancellationToken token)
    {
        var documentInput = new AddDocumentInput
        {
            Metadata = new MetadataInput
            {
                Text = text
            }
        };

        _logger.LogInformation("Sending document to API");
        var savedDocument = await _apiClient.AddDocument.ExecuteAsync(documentInput, token);

        if (savedDocument.IsErrorResult())
        {
            _logger.LogError("Error while saving document");
        }

        if (savedDocument.Data?.AddDocument.Document != null)
        {
            _logger.LogInformation("Document {DocumentId} saved", savedDocument.Data.AddDocument.Document.Id);
        }
    }

    private async Task<string> ReadDocumentTextAsync(
        MinioMessage uploadedDocumentMessage, 
        CancellationToken token)
    {
        var sb = new StringBuilder();
        foreach (var minioRecord in uploadedDocumentMessage.Records)
        {
            if (!TryGetDocumentReader(minioRecord, out var reader))
            {
                _logger.LogWarning(
                    "Unable to find a reader for file {FileName}",
                    minioRecord.Container.Object.Key);
                continue;
            }

            var url = await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(minioRecord.Container.Bucket.Name)
                .WithObject(minioRecord.Container.Object.Key)
                .WithExpiry((int)TimeSpan.FromHours(2).TotalSeconds));
            var dataStream = await new HttpClient().GetStreamAsync(new Uri(url), token);
            _logger.LogInformation(
                "Reading file {FileName}",
                minioRecord.Container.Object.Key);
            sb.AppendLine(await reader!.ReadDocumentAsync(dataStream, token));
        }

        var text = sb.ToString();
        return text;
    }

    private bool TryGetDocumentReader(MinioRecord minioRecord, out IDocumentReader? documentReader)
    {
        documentReader = _documentReaders
            .FirstOrDefault(reader => reader.SupportedContentTypes.Contains(minioRecord.Container.Object.ContentType));

        return documentReader != null;
    }
}