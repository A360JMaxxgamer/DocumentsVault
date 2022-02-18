using Files.Grpc;
using Files.Service.Handlers;
using Files.Service.Services.Exceptions;
using Google.Protobuf;
using Grpc.Core;

namespace Files.Service.Services;

public class DownloadService : Grpc.DownloadService.DownloadServiceBase
{
    private readonly IUploadIndexer _uploadIndexer;

    public DownloadService(IUploadIndexer uploadIndexer)
    {
        _uploadIndexer = uploadIndexer;
    }
    
    /// <inheritdoc />
    public override async Task<DocumentDownloadResult> DownloadDocument(
        DocumentDownloadRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.FileId, out var fileId))
        {
            throw new FileIdFormatException(request.FileId);
        }
        
        var uploadFile = await _uploadIndexer.GetAsync(fileId);
        
        ArgumentNullException.ThrowIfNull(uploadFile.FileName);
        
        var file = await File.ReadAllBytesAsync(uploadFile.FileName);

        return new DocumentDownloadResult
        {
            FileId = fileId.ToString(),
            Data = ByteString.CopyFrom(file)
        };
    }
}