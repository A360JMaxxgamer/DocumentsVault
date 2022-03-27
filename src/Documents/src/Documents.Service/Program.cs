using System.Diagnostics.CodeAnalysis;
using AspNetCore.Utilities.Configurations;
using Documents.Service.Configurations;
using Documents.Service.GraphQL;
using Documents.Service.Models;
using Documents.Service.Shared;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Documents.Service;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.BindConfiguration<DocumentServiceConfiguration>("DocumentService");
        builder.Services.AddSingleton<IMongoCollection<Document>>(provider =>
        {
            var fileServiceConfig = provider.GetRequiredService<DocumentServiceConfiguration>();
            return new MongoClient(fileServiceConfig.MongoConnectionString)
                .GetDatabase(ServiceConstants.FileIndexDb)
                .GetCollection<Document>(ServiceConstants.FileCollection);
        });

        builder.Services
            .AddSingleton(provider => ConnectionMultiplexer.Connect(provider.GetRequiredService<DocumentServiceConfiguration>().Redis.Host))
            .AddGraphQLServer()
            .AddMutationConventions()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddMongoDbProjections()
            .AddMongoDbFiltering()
            .AddMongoDbSorting()
            .AddMongoDbPagingProviders()
            .AddSorting()
            .InitializeOnStartup()
            .PublishSchemaDefinition(c => c
                .SetName("documents")
                .PublishToRedis("documentsVault", sp => sp.GetRequiredService<ConnectionMultiplexer>()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseRouting();
        app.MapGraphQL();

        app.Run();
    }
}