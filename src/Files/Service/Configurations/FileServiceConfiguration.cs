namespace Files.Service.Configurations;

public record FileServiceConfiguration
{
    public string Folder { get; set; } = "./Upload";
}