using Microsoft.Extensions.DependencyInjection;
using Serenity.AIHub.Client;
using Xunit;

namespace Serenity.AIHub.IntegrationTests;

public class ConversationIntegrationTests : IClassFixture<TestFixture>
{
    private readonly ISerenityAIHubClient _client;

    public ConversationIntegrationTests(TestFixture fixture)
    {
        _client = fixture.ServiceProvider.GetRequiredService<ISerenityAIHubClient>();
    }

    [Fact]
    public async Task CreateConversation_WithoutVersion_ShouldSucceed()
    {
        // Act
        var result = await _client.CreateConversation("assistantagent", null);

        // Assert
        Assert.NotEqual(Guid.Empty, result.ChatId);
        Assert.NotNull(result.Content);
        Assert.NotNull(result.ConversationStarters);
    }

    [Fact]
    public async Task CreateConversation_WithVersion_ShouldSucceed()
    {
        // Act
        var result = await _client.CreateConversation("assistantagent", 1);

        // Assert
        Assert.NotEqual(Guid.Empty, result.ChatId);
        Assert.NotNull(result.Content);
        Assert.NotNull(result.ConversationStarters);
        Assert.NotEmpty(result.ConversationStarters);
        Assert.Equal(1, result.Version);
    }

    [Fact]
    public async Task CreateConversation_WithInvalidAgent_ShouldFail()
    {
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _client.CreateConversation("invalid-agent", null));
    }
}