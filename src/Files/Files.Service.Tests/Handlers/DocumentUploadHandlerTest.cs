using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AspNetCore.Utilities.Configurations;
using Files.Grpc;
using Files.Service.Configurations;
using Files.Service.Handlers;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Files.Service.Tests.Handlers;

public class DocumentUploadHandlerTest : IDisposable
{
    private readonly string _tmpConfigFile;
    private readonly string _tmpPath;

    public DocumentUploadHandlerTest()
    {
        _tmpPath = Path.Combine(Path.GetTempPath(), "upload-test");
        _tmpConfigFile = Path.GetTempFileName();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Directory.Delete(_tmpPath, true);
        File.Delete(_tmpConfigFile);
    }

    [Fact]
    public async Task ShouldUploadFile()
    {
        // Arrange
        var services = new ServiceCollection();
        ConfigureServices(services);

        using var scope = services
            .BuildServiceProvider()
            .CreateScope();
        var documentHandler = scope
            .ServiceProvider
            .GetRequiredService<IDocumentUploadHandler>();
        var request = new DocumentUpload();
        request.Files.Add(new FileUpload
        {
            Filetype = "tmp",
            Filename = "Test",
            Data = ByteString.CopyFromUtf8("Test")
        });

        // Act
        var uploadResult = await documentHandler.UploadDocumentAsync(request);

        // Assert
        Assert.Single(uploadResult.UploadedFilesResult);
        Assert.Contains("tmp", uploadResult.UploadedFilesResult[0].FileName);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var json = $@"{{ ""fileService"": {{ ""folder"": ""{Regex.Escape(_tmpPath)}"" }} }}";
        File.WriteAllText(_tmpConfigFile, json);
        var config = new ConfigurationBuilder()
            .AddJsonFile(_tmpConfigFile)
            .Build();
        services.AddSingleton<IConfiguration>(config);
        services.AddTransient<IDocumentUploadHandler, DocumentUploadHandler>();
        services.BindConfiguration<FileServiceConfiguration>("fileService");
    }
}