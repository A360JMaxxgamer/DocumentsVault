using System.Diagnostics.CodeAnalysis;
using AspNetCore.Utilities.Configurations;
using Files.Service.Configurations;
using Files.Service.GraphQL;
using Files.Service.Handlers;
using Files.Service.Models;
using Files.Service.Services;
using Files.Service.Shared;
using MongoDB.Driver;
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
        builder.Services.AddTransient<IDocumentUploadHandler, DocumentUploadHandler>();
        builder.Services.AddTransient<IUploadIndexer, MongoIndexer>();
        builder.Services.AddTransient<IUploadPublisher, MessageQueuePublisher>();
        builder.Services.BindConfiguration<FileServiceConfiguration>("FileService");
        builder.Services.AddSingleton<IMongoCollection<UploadFile>>(provider =>
        {
            var fileServiceConfig = provider.GetRequiredService<FileServiceConfiguration>();
            return new MongoClient(fileServiceConfig.MongoConnectionString)
                .GetDatabase(ServiceConstants.FileIndexDb)
                .GetCollection<UploadFile>(ServiceConstants.FileCollection);
        });
        builder.Services
            .AddSingleton(ConnectionMultiplexer.Connect("localhost:7000"))
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMongoDbProjections()
            .AddMongoDbFiltering()
            .AddMongoDbSorting()
            .AddMongoDbPagingProviders()
            .AddSorting()
            .InitializeOnStartup()
            .PublishSchemaDefinition(c => c
                .SetName("files")
                .IgnoreRootTypes()
                .AddTypeExtensionsFromFile("./GraphQL/Stitching.graphql")
                .PublishToRedis("documentsVault", sp => sp.GetRequiredService<ConnectionMultiplexer>()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseRouting();
        app.MapGrpcService<UploadService>();
        app.MapGraphQL();

        app.Run();
    }
}