using System.Threading;
using System.Threading.Tasks;
using Files.Grpc;
using Files.Service.Handlers;
using Grpc.Core;
using Moq;
using UnitTestHelper.Mocks;
using Xunit;
using UploadService = Files.Service.Services.UploadService;

namespace Files.Service.Tests.Services
{
    public class UploadServiceTest
    {
        [Fact]
        public async Task ShouldReturnResultsForAllFiles()
        {
            // Arrange
            var uploadHandlerMock = new Mock<IDocumentUploadHandler>();
            uploadHandlerMock
                .Setup(_ => _.UploadDocumentAsync(It.IsAny<DocumentUpload>(), It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    var result = new DocumentUploadResult();
                    result.UploadedFilesResult.Add(new FileUploadResult());
                    return Task.FromResult(result);
                });
            var streamMock = new AsyncStreamReaderMock<DocumentUpload>(new DocumentUpload[]
            {
                new(),
                new()
            });

            var uploadService = new UploadService(uploadHandlerMock.Object);
            var ctxMock = new Mock<ServerCallContext>();

            // Act
            var result = await uploadService.UploadDocuments(streamMock, ctxMock.Object);

            Assert.Equal(2, result.Results.Count);
        }
    }
}