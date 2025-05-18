namespace Aish.Core.Models;

/// <summary>
/// Contains input and optional metadata passed to a command at execution time.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CommandContext"/> class.
/// </remarks>
/// <param name="input">The raw input string.</param>
/// <param name="metadata">Optional key-value metadata dictionary.</param>
public sealed class CommandContext(string input, IDictionary<string, object>? metadata = null)
{
	/// <summary>
	/// Gets the raw input string provided by the user.
	/// </summary>
	public string Input { get; init; } = input;

	/// <summary>
	/// Gets optional metadata to assist command execution (e.g. user, source, options).
	/// </summary>
	public IDictionary<string, object>? Metadata { get; init; } = metadata;
}