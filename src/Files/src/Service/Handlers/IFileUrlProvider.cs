using Files.Service.Models;

namespace Files.Service.Handlers;

public interface IFileUrlProvider
{
    Task<PreSignedUrl> CreatePreSignedUploadUrl(string fileName);
    Task<PreSignedUrl> GetPreSignedDownloadUrl(Guid documentId);
}