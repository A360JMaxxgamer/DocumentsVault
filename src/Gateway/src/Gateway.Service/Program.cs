using StackExchange.Redis;

namespace Gateway.Service;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AddFederatedSchemaClient(builder.Services, "files");
        AddFederatedSchemaClient(builder.Services, "documents");
        
        builder.Services
            .AddSingleton(provider => ConnectionMultiplexer.Connect(provider.GetRequiredService<IConfiguration>().GetValue<string>("redis")))
            .AddGraphQLServer()
            .AddRemoteSchemasFromRedis("documentsVault", sp => sp.GetRequiredService<ConnectionMultiplexer>());
        
        var app = builder.Build();
        app.MapGraphQL();
        app.Run();
    }

    private static void AddFederatedSchemaClient(IServiceCollection services, string schemaName) =>
        services.AddHttpClient(schemaName, (provider, client) =>
        {
            var address = provider
                .GetRequiredService<IConfiguration>()
                .GetValue<string>(schemaName);
            client.BaseAddress = new Uri(address);
        });
}