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

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    /// <inheritdoc />
    public async Task<CreateConversationRes> CreateConversation(string agentCode, int? version, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(agentCode))
            throw new ArgumentNullException(nameof(agentCode));

        var queryString = version.HasValue ? $"?version={version}" : string.Empty;
        var response = await _httpClient.PostAsync($"/v1/conversations/{agentCode}{queryString}", null, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CreateConversationRes>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}