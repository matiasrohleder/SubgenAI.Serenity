using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serenity.AIHub.Abstractions;

namespace Serenity.AIHub.Client;

public class SerenityAIHubClient : ISerenityAIHubClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SerenityAIHubClient(string apiKey, HttpClient httpClient)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(content)!;
    }
}