namespace Files.Worker.Analysis;

public interface IDocumentReader
{
    string[] SupportedContentTypes { get; }
    
    Task<string> ReadDocumentAsync(Stream data, CancellationToken token);
}