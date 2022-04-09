using Files.Worker.Models;

namespace Files.Worker.Analysis;

/// <summary>
/// Analyzer for files which is responsible for uploading the results to document service
/// </summary>
public interface IDocumentAnalyzer
{
    Task AnalyzeDocumentAsync(MinioMessage minioMessage, CancellationToken token);
}