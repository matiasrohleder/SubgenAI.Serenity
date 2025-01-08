using Serenity.AIHub.Models;

namespace Serenity.AIHub.Client;

/// <inheritdoc />
public class SerenityAIHubClient : ISerenityAIHubClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerenityAIHubClient"/> class.
    /// </summary>
    /// <param name="apiKey">The API key to use for authentication.</param>
    /// <param name="httpClient">The HTTP client to use for making requests.</param>
    public SerenityAIHubClient(string apiKey, HttpClient httpClient)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    /// <inheritdoc />
    public Task<CreateConversationRes> CreateConversation(string agentCode, int? version, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}