using System.Collections.Generic;
using System.Threading.Tasks;
using Files.Service.GraphQL;
using Files.Service.Handlers;
using Files.Service.Models;
using Moq;
using UnitTestHelper.Extensions;
using Xunit;

namespace Files.Service.Tests.GraphQL;

public class MutationTest
{
    [Fact]
    public async Task CreatePreSignedUploadUrl_Returns_Url()
    {
        // Arrange
        var fileUrlProvider = new Mock<IFileUrlProvider>();
        fileUrlProvider
            .SetupReturn(f => f.CreatePreSignedUploadUrls(It.IsAny<string[]>()), Task.FromResult(new List<PreSignedUrl>
            {
                new (){FileName = "One", Url = "http://bla/bucket/one"},
                new (){FileName = "Two", Url = "http://bla/bucket/two"}
            }));
        var mutation = new Mutation();

        // Act
        var result = await mutation.CreatePreSignedUploadUrl(
            fileUrlProvider.Object,
            new []{"One, Two"});

        // Assert
        Assert.Equal(2, result.Count);
    }
}