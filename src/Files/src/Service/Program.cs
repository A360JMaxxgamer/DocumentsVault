using System.Diagnostics.CodeAnalysis;
using AspNetCore.Utilities.Configurations;
using Files.Service.Configurations;
using Files.Service.GraphQL;
using Files.Service.Handlers;
using StackExchange.Redis;

namespace Files.Service;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddTransient<IFileUrlProvider, FileUrlProvider>();
        builder.Services.AddTransient<IFilePublisher, NatsFilePublisher>();
        builder.Services.BindConfiguration<FileServiceConfiguration>("FileService");

        builder.Services
            .AddSingleton(provider => ConnectionMultiplexer.Connect(provider.GetRequiredService<IConfiguration>().GetValue<string>("redis")))
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddMutationConventions()
            .InitializeOnStartup()
            .PublishSchemaDefinition(c => c
                .SetName("files")
                .AddTypeExtensionsFromFile("./GraphQL/Stitching.graphql")
                .PublishToRedis("documentsVault", sp => sp.GetRequiredService<ConnectionMultiplexer>()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseRouting();
        app.MapGraphQL();

        app.Run();
    }
}