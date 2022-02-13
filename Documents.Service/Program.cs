using System.Diagnostics.CodeAnalysis;
using Documents.Service.GraphQL;
using StackExchange.Redis;

namespace Documents.Service;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddSingleton(ConnectionMultiplexer.Connect("localhost:7000"))
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
                .IgnoreRootTypes()
                .AddTypeExtensionsFromFile("./GraphQL/Stitching.graphql")
                .PublishToRedis("documentsVault", sp => sp.GetRequiredService<ConnectionMultiplexer>()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseRouting();
        app.MapGraphQL();

        app.Run();
    }
}