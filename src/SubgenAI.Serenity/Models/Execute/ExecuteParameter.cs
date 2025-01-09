namespace SubgenAI.Serenity.Models.Execute;

/// <summary>
/// Represents a parameter for the execute endpoint.
/// </summary>
public class ExecuteParameter
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