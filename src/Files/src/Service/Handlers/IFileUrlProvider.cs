using Files.Service.Models;

namespace Files.Service.Handlers;

public interface IFileUrlProvider
{
    Task<List<PreSignedUrl>> CreatePreSignedUploadUrls(string[] fileNames);
    Task<IEnumerable<PreSignedUrl>> GetPreSignedDownloadUrls(Guid documentId);
}