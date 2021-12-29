using System.IO;
using AspNetCore.Utilities.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspNetCore.Utilities.Tests.Configurations;

public class ConfigurationExtensionsTest
{
    [Fact]
    public void ShouldBindConfig()
    {
        // Arrange
        const string json = @"
{
  ""test"": {
    ""id"": 2,
    ""name"": ""test""
  }  
}
";
        using var memStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memStream);
        streamWriter.Write(json);
        streamWriter.Flush();

        memStream.Seek(0, 0);
        var config = new ConfigurationBuilder()
            .AddJsonStream(memStream)
            .Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.BindConfiguration<TestConfig>("test");
        var provider = services.BuildServiceProvider();
        
        // Act
        var testConfig = provider.GetRequiredService<TestConfig>();

        // Assert
        Assert.Equal(2, testConfig.Id);
        Assert.Equal("test", testConfig.Name);
    }
}

public record TestConfig
{
    public int Id { get; set; } = 0;

    public string Name { get; set; } = "Default";
}