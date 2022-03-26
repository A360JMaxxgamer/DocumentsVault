namespace Files.Worker.Analysis;

/// <summary>
/// Analyzer for files which is responsible for uploading the results to document service
/// </summary>
public interface IFileAnalyzer
{
    Task AnalyzeFileAsync(Guid fileId, Stream data);
}