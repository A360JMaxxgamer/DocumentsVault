using Files.Service.Handlers;
using Files.Service.Models;

namespace Files.Service.GraphQL;

public class Mutation
{
    public Task<PreSignedUrl> CreatePreSignedUploadUrl([Service] IFileUrlProvider fileUrlProvider, string fileName) =>
        fileUrlProvider.CreatePreSignedUploadUrl(fileName);
    
    [UseMutationConvention(PayloadFieldName = "triggerDate")]
    public async Task<DateTime> TriggerUploadedFilesIndexing([Service] IFilePublisher filePublisher)
    {
        await filePublisher.PublishUploadAsync();
        return DateTime.UtcNow;
    }
}