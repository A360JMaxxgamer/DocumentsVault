namespace Files.Worker.Analysis;

public interface IDocumentReader
{
    string[] SupportedFileExtensions { get; }
    
    Task<AddDocumentInput> ReadDocumentAsync(Stream data, CancellationToken token);
}