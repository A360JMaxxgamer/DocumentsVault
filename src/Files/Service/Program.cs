using AspNetCore.Utilities.Configurations;
using Files.Service.Configurations;
using Files.Service.Handlers;
using Files.Service.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddTransient<IDocumentUploadHandler, DocumentUploadHandler>();
builder.Services.BindConfiguration<FileServiceConfiguration>("FileService");
builder.Services.AddTransient<IMongoClient>(provider =>
{
    var fileServiceConfig = provider.GetRequiredService<FileServiceConfiguration>();
    return new MongoClient(fileServiceConfig.MongoConnectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UploadService>();

app.Run();