namespace Serenity.AIHub.Models;

/// <summary>
/// Represents a key-value pair for the message request.
/// </summary>
public class MessageParameter
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response from sending a message.
/// </summary>
public class SendMessageRes
{
    /// <summary>
    /// Gets or sets the response content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}