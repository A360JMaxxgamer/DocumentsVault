using Files.Service.Configurations;
using Files.Service.Models;
using Minio;

namespace Files.Service.Handlers;

internal class FileUrlProvider : IFileUrlProvider
{
    private readonly FileServiceConfiguration _fileServiceConfiguration;

    public FileUrlProvider(FileServiceConfiguration fileServiceConfiguration)
    {
        _fileServiceConfiguration = fileServiceConfiguration;
    }

    /// <inheritdoc />
    public async Task<PreSignedUrl> CreatePreSignedUploadUrl(string fileName)
    {
        var uploadArgs = new PresignedPutObjectArgs()
            .WithBucket(_fileServiceConfiguration.MinioSettings.UploadBucket)
            .WithObject(fileName)
            .WithExpiry((int) TimeSpan.FromHours(2).TotalSeconds);

        var preSignedUrl = await GetMinioClient()
            .PresignedPutObjectAsync(uploadArgs);

        return new PreSignedUrl
        {
            Url = preSignedUrl
        };
    }

    /// <inheritdoc />
    public async Task<PreSignedUrl> GetPreSignedDownloadUrl(Guid documentId)
    {
        var minioClient = GetMinioClient()
            .WithCredentials(
                _fileServiceConfiguration.MinioSettings.Username,
                _fileServiceConfiguration.MinioSettings.Password)
            .WithEndpoint(_fileServiceConfiguration.MinioSettings.Endpoint)
            .Build();
        var getPreSignedArgs = new PresignedGetObjectArgs()
            .WithBucket(_fileServiceConfiguration.MinioSettings.UploadBucket)
            .WithObject(documentId.ToString())
            .WithExpiry((int) TimeSpan.FromDays(2).TotalSeconds);

        var preSignedUrl = await minioClient.PresignedGetObjectAsync(getPreSignedArgs);
        return new PreSignedUrl
        {
            Url = preSignedUrl
        };
    }

    private MinioClient GetMinioClient() =>
        new MinioClient()
            .WithCredentials(
                _fileServiceConfiguration.MinioSettings.Username,
                _fileServiceConfiguration.MinioSettings.Password)
            .WithEndpoint(_fileServiceConfiguration.MinioSettings.Endpoint)
            .Build();
}