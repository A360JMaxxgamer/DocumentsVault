using AspNetCore.Utilities.Configurations;
using Files.Service.Configurations;
using Files.Service.Handlers;
using Files.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddTransient<IDocumentUploadHandler, DocumentUploadHandler>();
builder.Services.BindConfiguration<FileServiceConfiguration>("FileService");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UploadService>();

app.Run();