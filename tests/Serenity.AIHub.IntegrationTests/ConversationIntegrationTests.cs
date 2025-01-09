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

    [Fact]
    public async Task CreateConversationAndSendMessage_ShouldSucceed()
    {
        // Arrange - Create a conversation first
        var conversation = await _client.CreateConversation("assistantagent", null);
        Assert.NotEqual(Guid.Empty, conversation.ChatId);

        // Act - Send a message to the conversation
        var messageResponse = await _client.SendMessage(
            "assistantagent",
            conversation.ChatId,
            "Hello, how are you?");

        // Assert
        Assert.NotNull(messageResponse);
        Assert.NotNull(messageResponse.Content);
        Assert.NotEmpty(messageResponse.Content);
    }

    [Fact]
    public async Task SendMessage_WithInvalidChatId_ShouldFail()
    {
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _client.SendMessage("assistantagent", Guid.NewGuid(), "Hello"));
    }

    [Fact]
    public async Task SendMessage_WithEmptyChatId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _client.SendMessage("assistantagent", Guid.Empty, "Hello"));
    }

    [Fact]
    public async Task SendMessage_WithEmptyMessage_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _client.SendMessage("assistantagent", Guid.NewGuid(), string.Empty));
    }

    [Fact]
    public async Task FullConversationFlow_ShouldSucceed()
    {
        // Arrange - Create a conversation
        var conversation = await _client.CreateConversation("assistantagent", null);
        Assert.NotEqual(Guid.Empty, conversation.ChatId);

        // Act & Assert - Send multiple messages
        var messages = new[]
        {
            "Hello, how are you?",
            "What can you help me with?",
            "Thank you for your help!"
        };

        foreach (var message in messages)
        {
            var response = await _client.SendMessage(
                "assistantagent",
                conversation.ChatId,
                message);

            Assert.NotNull(response);
            Assert.NotNull(response.Content);
            Assert.NotEmpty(response.Content);

            // Add a small delay between messages to avoid rate limiting
            await Task.Delay(1000);
        }
    }
}