using System.Text.RegularExpressions;
using StrawberryShake;

namespace Files.Worker.Analysis;

internal class FileAnalyzer : IFileAnalyzer
{
    private readonly IApiClient _apiClient;
    private readonly IEnumerable<IDocumentReader> _documentReaders;
    private readonly ILogger<FileAnalyzer> _logger;
    private readonly Regex _extensionRegex = new Regex(@"^[^?]*", RegexOptions.Compiled);

    public FileAnalyzer(
        IApiClient apiClient,
        IEnumerable<IDocumentReader> documentReaders,
        ILogger<FileAnalyzer> logger)
    {
        _apiClient = apiClient;
        _documentReaders = documentReaders;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task AnalyzeFileAsync(string fileUrl, CancellationToken token)
    {
        var fileExtension = GetFileExtension(fileUrl);
        var documentReader = _documentReaders
            .FirstOrDefault(reader => reader.SupportedFileExtensions.Contains(fileExtension));
        
        if (documentReader == null)
        {
            _logger.LogWarning("File extension {FileExtension} is not supported", fileExtension);
            return;
        }
        
        var dataStream = await new HttpClient().GetStreamAsync(new Uri(fileUrl), token);
        
        _logger.LogInformation("Analyzing file {FileUrl}", fileUrl);
        var document = await documentReader.ReadDocumentAsync(dataStream, token);
        
        _logger.LogInformation("Sending document to API");
        var savedDocument = await _apiClient.AddDocument.ExecuteAsync(document, token);

        if (savedDocument.IsErrorResult())
        {
            _logger.LogError("Error while saving document");
        }

        if (savedDocument.Data?.AddDocument.Document != null)
        {
            _logger.LogInformation("Document {DocumentId} saved", savedDocument.Data.AddDocument.Document.Id);
        }
    }
    
    private string GetFileExtension(string fileUrl)
    {
        var fileName = Path.GetFileName(fileUrl);
        var extension = Path.GetExtension(fileName);
        return _extensionRegex.Match(extension).Value;
    }
}