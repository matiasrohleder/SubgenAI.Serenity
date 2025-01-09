using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Serenity.AIHub.Client;
using Serenity.AIHub.Extensions;
using Serenity.AIHub.Models;
using Xunit;

namespace Serenity.AIHub.Tests;

public class SerenityAIHubClientTests
{
    [Fact]
    public async Task CreateConversation_ValidRequest_ReturnsExpectedResponse()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(async request =>
        {
            // Verify request
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/v2/agent/assistantagent/conversation?version=1", request.RequestUri?.PathAndQuery);
            Assert.Equal("b87020eb-f3d0-4ef4-96ce-5ca60ca5e652", request.Headers.GetValues("X-API-KEY").FirstOrDefault());
            Assert.Equal("application/json", request.Headers.GetValues("Content-Type").FirstOrDefault());

            // Return mock response
            var response = new CreateConversationRes
            {
                ChatId = Guid.NewGuid(),
                Content = "Hello!",
                ConversationStarters = new List<string> { "Hi", "Hello" },
                Version = 1,
                UseVision = false
            };

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(response))
            };
        });

        var services = new ServiceCollection();
        services.AddSerenityAIHub(options =>
        {
            options.ApiKey = "b87020eb-f3d0-4ef4-96ce-5ca60ca5e652";
            options.BaseUrl = "https://test-api-aihub.binit.dev";
        });

        // Replace the HttpClient with our test handler
        services.AddHttpClient<ISerenityAIHubClient, SerenityAIHubClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ISerenityAIHubClient>();

        // Act
        var result = await client.CreateConversation("assistantagent", 1);

        // Assert
        Assert.NotEqual(Guid.Empty, result.ChatId);
        Assert.Equal("Hello!", result.Content);
        Assert.Contains("Hi", result.ConversationStarters);
        Assert.Equal(1, result.Version);
        Assert.False(result.UseVision);
    }

    [Fact]
    public async Task CreateConversation_NoVersion_OmitsVersionParameter()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(async request =>
        {
            // Verify request
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/v2/agent/assistantagent/conversation", request.RequestUri?.PathAndQuery);
            Assert.Equal("b87020eb-f3d0-4ef4-96ce-5ca60ca5e652", request.Headers.GetValues("X-API-KEY").FirstOrDefault());
            Assert.Equal("application/json", request.Headers.GetValues("Content-Type").FirstOrDefault());

            var response = new CreateConversationRes
            {
                ChatId = Guid.NewGuid(),
                Content = "Hello!",
                ConversationStarters = new List<string> { "Hi", "Hello" },
                Version = 1,
                UseVision = false
            };

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(response))
            };
        });

        var services = new ServiceCollection();
        services.AddSerenityAIHub(options =>
        {
            options.ApiKey = "b87020eb-f3d0-4ef4-96ce-5ca60ca5e652";
            options.BaseUrl = "https://test-api-aihub.binit.dev";
        });

        services.AddHttpClient<ISerenityAIHubClient, SerenityAIHubClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ISerenityAIHubClient>();

        // Act
        var result = await client.CreateConversation("assistantagent", null);

        // Assert
        Assert.NotEqual(Guid.Empty, result.ChatId);
    }
}