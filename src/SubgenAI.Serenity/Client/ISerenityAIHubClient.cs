using SubgenAI.Serenity.Models;
using SubgenAI.Serenity.Models.Execute;

namespace SubgenAI.Serenity.Client;

/// <summary>
/// Represents the interface for interacting with the Serenity AI Hub.
/// </summary>
public interface ISerenityAIHubClient
{
    /// <summary>
    /// Creates a conversation with the specified agent code and version.
    /// </summary>
    /// <param name="agentCode">The agent code to use for the conversation.</param>
    /// <param name="version">The version of the agent.</param>
    /// <param name="apiVersion">The API version to use (default is 2).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created conversation response.</returns>
    Task<CreateConversationRes> CreateConversation(string agentCode, int? version = null, int apiVersion = 2, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an agent.
    /// </summary>
    /// <param name="agentCode">The agent code.</param>
    /// <param name="input">Agent execution inputs</param>
    /// <param name="apiVersion">The API version to use (default is 2).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the agent.</returns>
    Task<AgentResult> Execute(string agentCode, List<ExecuteParameter> input = null, int apiVersion = 2, CancellationToken cancellationToken = default);
}