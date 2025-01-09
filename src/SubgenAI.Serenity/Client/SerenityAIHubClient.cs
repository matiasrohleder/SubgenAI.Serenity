using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SubgenAI.Serenity.Constants;
using SubgenAI.Serenity.Models;
using SubgenAI.Serenity.Models.Execute;

namespace SubgenAI.Serenity.Client;

/// <inheritdoc />
public class SerenityAIHubClient : ISerenityAIHubClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="SerenityAIHubClient"/> class for dependency injection.
    /// </summary>
    /// <param name="options">The options containing the API key.</param>
    /// <param name="httpClient">The HTTP client to use for making requests.</param>
    public SerenityAIHubClient(IOptions<SerenityAIHubOptions> options, HttpClient httpClient)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        var apiKey = options.Value.ApiKey ?? throw new ArgumentNullException(nameof(options.Value.ApiKey));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        ConfigureHttpClient(_httpClient, apiKey);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerenityAIHubClient"/> class for direct instantiation.
    /// </summary>
    /// <param name="apiKey">The API key to use for authentication.</param>
    public static SerenityAIHubClient Create(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));

        var httpClient = new HttpClient();
        ConfigureHttpClient(httpClient, apiKey);

        return new SerenityAIHubClient(httpClient);
    }

    // Private constructor for direct instantiation
    private SerenityAIHubClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private static void ConfigureHttpClient(HttpClient client, string apiKey)
    {
        client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        client.BaseAddress = new Uri(ClientConstants.BaseUrl);
    }

    /// <inheritdoc />
    public async Task<CreateConversationRes> CreateConversation(string agentCode, int? version, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(agentCode))
            throw new ArgumentNullException(nameof(agentCode));

        var queryString = version.HasValue ? $"?version={version}" : string.Empty;

        // Create an empty content with the correct Content-Type header
        var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"/api/v2/agent/{agentCode}/conversation{queryString}", content, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CreateConversationRes>(JsonOptions, cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <inheritdoc />
    public async Task<AgentResult> SendMessage(string agentCode, Guid chatId, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(agentCode))
            throw new ArgumentNullException(nameof(agentCode));
        if (chatId == Guid.Empty)
            throw new ArgumentException("Chat ID cannot be empty", nameof(chatId));
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        var parameters = new[]
        {
            new ExecuteParameter { Key = "chatId", Value = chatId.ToString() },
            new ExecuteParameter { Key = "message", Value = message }
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v2/agent/{agentCode}/execute",
            parameters,
            JsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AgentResult>(JsonOptions, cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}