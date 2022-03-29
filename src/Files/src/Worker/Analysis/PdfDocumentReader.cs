using System.Text;
using UglyToad.PdfPig;

namespace Files.Worker.Analysis;

internal class PdfDocumentReader : IDocumentReader
{
    /// <inheritdoc />
    public string[] SupportedContentTypes => new[] { "application/pdf" };

    /// <inheritdoc />
    public Task<string> ReadDocumentAsync(Stream data, CancellationToken token)
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


        return Task.FromResult(sb.ToString());
    }
}