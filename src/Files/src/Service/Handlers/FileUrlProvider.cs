using System.Reactive.Linq;
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
    public async Task<IEnumerable<PreSignedUrl>> GetPreSignedDownloadUrls(Guid documentId)
    {
        var minioClient = GetMinioClient()
            .WithCredentials(
                _fileServiceConfiguration.MinioSettings.Username,
                _fileServiceConfiguration.MinioSettings.Password)
            .WithEndpoint(_fileServiceConfiguration.MinioSettings.Endpoint)
            .Build();

        var listObjectsArgs = new ListObjectsArgs()
            .WithBucket(documentId.ToString());

        var files = await minioClient
            .ListObjectsAsync(listObjectsArgs)
            .ToList();
        var urls = await Task.WhenAll(files
            .Select(file => new PresignedGetObjectArgs()
                .WithBucket(documentId.ToString())
                .WithObject(file.Key)
                .WithExpiry((int) TimeSpan.FromDays(2).TotalSeconds))
            .Select(args => minioClient.PresignedGetObjectAsync(args))
            .ToList());
        
        return urls
            .Select(url => new PreSignedUrl
            {
                Url = url
            })
            .ToArray();
    }

    private MinioClient GetMinioClient() =>
        new MinioClient()
            .WithCredentials(
                _fileServiceConfiguration.MinioSettings.Username,
                _fileServiceConfiguration.MinioSettings.Password)
            .WithEndpoint(_fileServiceConfiguration.MinioSettings.Endpoint)
            .Build();
}