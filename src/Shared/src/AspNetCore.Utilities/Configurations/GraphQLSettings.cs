namespace AspNetCore.Utilities.Configurations;

public record GraphQLSettings
{
    public string ApiGatewayEndpoint { get; init; } = "http://localhost:5267/graphql";
}