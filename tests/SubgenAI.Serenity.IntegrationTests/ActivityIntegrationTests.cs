using Microsoft.Extensions.DependencyInjection;
using SubgenAI.Serenity.Client;
using SubgenAI.Serenity.Models.Execute;
using Xunit;

namespace SubgenAI.Serenity.IntegrationTests;

public class ActivityIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly ISerenityAIHubClient _client = fixture.ServiceProvider.GetRequiredService<ISerenityAIHubClient>();

    [Fact]
    public async Task Execute_WithWord_ShouldSucceed()
    {
        // Arrange
        List<ExecuteParameter> input =
        [
            new("word", "running")
        ];

        // Act
        AgentResult result = await _client.Execute("activityagent", input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        Assert.NotEmpty(result.Content);
        Assert.NotEqual(Guid.Empty, result.InstanceId);
    }

    [Fact]
    public async Task Execute_WithoutWord_ShouldFail()
    {
        // Act
        HttpRequestException exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
            _client.Execute("activityagent"));

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("Request failed with status code", exception.Message);
    }

    [Fact]
    public async Task Execute_WithDifferentWords_ShouldReturnDifferentResponses()
    {
        // Arrange
        string[] words = { "swimming", "cycling", "hiking" };
        var responses = new List<string>();

        // Act
        foreach (string word in words)
        {
            List<ExecuteParameter> input =
            [
                new("word", word)
            ];

            AgentResult result = await _client.Execute("activityagent", input);

            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);

            responses.Add(result.Content);

            // Add a small delay between requests to avoid rate limiting
            await Task.Delay(1000);
        }

        // Assert
        Assert.Equal(words.Length, responses.Distinct().Count());
    }

    [Fact]
    public async Task Execute_WithVersion_ShouldSucceed()
    {
        // Arrange
        List<ExecuteParameter> input =
        [
            new("word", "dancing")
        ];

        // Act
        AgentResult result = await _client.Execute("activityagent", input, agentVersion: 25);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        Assert.NotEmpty(result.Content);
        Assert.NotEqual(Guid.Empty, result.InstanceId);
    }

    [Fact]
    public async Task Execute_WithActionResults_ShouldReturnValidData()
    {
        // Arrange
        List<ExecuteParameter> input =
        [
            new("word", "yoga")
        ];

        // Act
        AgentResult result = await _client.Execute("activityagent", input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        Assert.NotEmpty(result.Content);

        if (result.ActionResults != null)
        {
            Assert.IsType<Dictionary<string, object>>(result.ActionResults);
        }

        if (result.ExecutorTaskLogs != null)
        {
            Assert.IsType<List<ExecutorTaskResult>>(result.ExecutorTaskLogs);
        }
    }
}