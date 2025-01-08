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
            Assert.Equal("/v1/conversations/test-agent?version=1", request.RequestUri?.PathAndQuery);
            Assert.Equal("Bearer test-key", request.Headers.Authorization?.ToString());

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
            options.ApiKey = "test-key";
            options.BaseUrl = "https://api.serenityaihub.com";
        });

        // Replace the HttpClient with our test handler
        services.AddHttpClient<ISerenityAIHubClient, SerenityAIHubClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ISerenityAIHubClient>();

        // Act
        var result = await client.CreateConversation("test-agent", 1);

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
            Assert.Equal("/v1/conversations/test-agent", request.RequestUri?.PathAndQuery);
            Assert.Equal("Bearer test-key", request.Headers.Authorization?.ToString());

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
            options.ApiKey = "test-key";
            options.BaseUrl = "https://api.serenityaihub.com";
        });

        services.AddHttpClient<ISerenityAIHubClient, SerenityAIHubClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ISerenityAIHubClient>();

        // Act
        var result = await client.CreateConversation("test-agent", null);

        // Assert
        Assert.NotEqual(Guid.Empty, result.ChatId);
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public TestHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return await _handler(request);
        }
    }
}