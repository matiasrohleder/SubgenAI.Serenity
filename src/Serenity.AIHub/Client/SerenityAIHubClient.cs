using System.Net.Http.Json;
using Microsoft.Extensions.Options;
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
    /// <param name="options">The options containing the API key.</param>
    /// <param name="httpClient">The HTTP client to use for making requests.</param>
    public SerenityAIHubClient(IOptions<SerenityAIHubOptions> options, HttpClient httpClient)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        _apiKey = options.Value.ApiKey ?? throw new ArgumentNullException(nameof(options.Value.ApiKey));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
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

        return await response.Content.ReadFromJsonAsync<CreateConversationRes>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <inheritdoc />
    public async Task<SendMessageRes> SendMessage(string agentCode, Guid chatId, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(agentCode))
            throw new ArgumentNullException(nameof(agentCode));
        if (chatId == Guid.Empty)
            throw new ArgumentException("Chat ID cannot be empty", nameof(chatId));
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        var parameters = new[]
        {
            new MessageParameter { Key = "chatId", Value = chatId.ToString() },
            new MessageParameter { Key = "message", Value = message }
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v2/agent/{agentCode}/execute",
            parameters,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SendMessageRes>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}