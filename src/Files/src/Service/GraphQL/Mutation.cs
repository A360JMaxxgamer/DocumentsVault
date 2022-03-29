using Files.Service.Handlers;
using Files.Service.Models;

namespace Files.Service.GraphQL;

public class Mutation
{
    public Task<List<PreSignedUrl>> CreatePreSignedUploadUrl([Service] IFileUrlProvider fileUrlProvider, string[] filenames) =>
        fileUrlProvider.CreatePreSignedUploadUrls(filenames);
}