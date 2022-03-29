using AspNetCore.Utilities.Configurations;
using Files.Worker;
using Files.Worker.Analysis;
using Files.Worker.Configurations;
using Minio;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddApiClient()
            .ConfigureHttpClient((sp, client) =>
            {
                client.BaseAddress = new Uri(sp
                    .GetRequiredService<FilesWorkerConfiguration>()
                    .GraphQlSettings
                    .ApiGatewayEndpoint);
            });
        services.AddTransient<IDocumentAnalyzer, DocumentAnalyzer>();
        services.AddTransient<IDocumentReader, PdfDocumentReader>();
        services.AddTransient<IMinioClient>(sp =>
        {
            var config = sp.GetRequiredService<FilesWorkerConfiguration>();
            var mongoClient = new MinioClient()
                .WithEndpoint(config.Minio.Endpoint)
                .WithCredentials(config.Minio.Username, config.Minio.Password);
            return mongoClient;
        });
        services.BindConfiguration<FilesWorkerConfiguration>("FileWorker");
    })
    .Build();

await host.RunAsync();