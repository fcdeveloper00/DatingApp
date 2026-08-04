namespace API.Helpers;

public class CloudinarySettings
{
    // The prop names need to match what you have inside your configuration file.
    public required string CloudName { get; set; }
    public required string ApiKey {get;set;}
    public required string ApiSecret {get;set;}
}
