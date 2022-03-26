using AspNetCore.Utilities.Configurations;
using Files.Worker;
using Files.Worker.Analysis;
using Files.Worker.Configurations;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<IFileAnalyzer, FileAnalyzer>();
        services.BindConfiguration<FilesWorkerConfiguration>("FileWorker");
    })
    .Build();

await host.RunAsync();