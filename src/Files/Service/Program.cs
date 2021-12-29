using Files.Service.Handlers;
using Files.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddTransient<IDocumentUploadHandler, DocumentUploadHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UploadService>();

app.Run();