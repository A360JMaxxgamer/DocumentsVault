using System.Text;
using UglyToad.PdfPig;

namespace Files.Worker.Analysis;

internal class PdfDocumentReader : IDocumentReader
{
    /// <inheritdoc />
    public string[] SupportedFileExtensions => new[] { ".pdf" };

    /// <inheritdoc />
    public Task<AddDocumentInput> ReadDocumentAsync(Stream data, CancellationToken token)
    {
        using var memoryStream = new MemoryStream();
        data.CopyTo(memoryStream);
        memoryStream.Seek(0, 0);
        
        using var pdfDocument = PdfDocument.Open(memoryStream);

        var pageContents = pdfDocument
            .GetPages()
            .Select(page => page.Text)
            .ToArray();

        var sb = new StringBuilder();
        foreach (var pageContent in pageContents)
        {
            sb.AppendLine(pageContent);
        }
        
        var addDocumentInput = new AddDocumentInput
        {
            Metadata = new MetadataInput
            {
                Title = string.Empty,
                Tags = new List<string>(),
                Text = sb.ToString()
            }
        };
        return Task.FromResult(addDocumentInput);
    }
}