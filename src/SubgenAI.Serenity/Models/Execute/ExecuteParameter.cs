namespace SubgenAI.Serenity.Models.Execute;

/// <summary>
/// Represents a parameter for the execute endpoint.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExecuteParameter"/> class.
/// </remarks>
/// <param name="key">The key of the parameter.</param>
/// <param name="value">The value of the parameter.</param>
public class ExecuteParameter(string key, object value)
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public string Key { get; } = key ?? throw new ArgumentNullException(nameof(key));

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public object Value { get; } = value ?? throw new ArgumentNullException(nameof(value));
}