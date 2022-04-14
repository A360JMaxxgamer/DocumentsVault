using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Utilities.Configurations;

[ExcludeFromCodeCoverage]
public record GraphQLSettings
{
    public string ApiGatewayEndpoint { get; init; } = "http://localhost:5267/graphql";
}