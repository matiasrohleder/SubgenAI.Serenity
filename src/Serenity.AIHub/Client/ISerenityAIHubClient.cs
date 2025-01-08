using Serenity.AIHub.Models;

namespace Serenity.AIHub.Client;

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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created conversation response.</returns>
    Task<CreateConversationRes> CreateConversation(string agentCode, int? version, CancellationToken cancellationToken = default);
}