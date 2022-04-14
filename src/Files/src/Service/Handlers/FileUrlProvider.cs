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
    public async Task<List<PreSignedUrl>> CreatePreSignedUploadUrls(string[] fileNames)
    {
        var client = GetMinioClient();
        var preSignedUrls = new List<PreSignedUrl>();
        var uploadId = Guid.NewGuid();
        var bucketName = _fileServiceConfiguration.MinioSettings.UploadBucket;
        if (! await client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
        {
            await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }

        foreach (var fileName in fileNames)
        {
            var uploadPath = uploadId + "/" + fileName;
            var args = new PresignedPutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(uploadPath)
                .WithExpiry((int) TimeSpan.FromHours(2).TotalSeconds);
            var preSignedUrl = await client.PresignedPutObjectAsync(args);
            preSignedUrls.Add(new PreSignedUrl
            {
                FileName = fileName,
                Url = preSignedUrl
            });
        }

        return preSignedUrls;
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