namespace Serenity.AIHub.Models;

public class SerenityAIHubOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.serenityaihub.com";
    public int TimeoutSeconds { get; set; } = 30;
}