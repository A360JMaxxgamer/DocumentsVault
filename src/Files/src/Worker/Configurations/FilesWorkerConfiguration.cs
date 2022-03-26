using AspNetCore.Utilities.Configurations;

namespace Files.Worker.Configurations;

public record FilesWorkerConfiguration
{
    public NatsSettings Nats { get; init; } = new();

    public GraphQLSettings GraphQlSettings { get; init; } = new();
}