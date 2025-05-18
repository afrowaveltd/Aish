namespace Aish.Core.Enums;

/// <summary>
/// Represents the result of executing a command.
/// </summary>
public enum CommandResult
{
	/// <summary>
	/// The command executed successfully.
	/// </summary>
	Success,

	/// <summary>
	/// The command failed to execute correctly.
	/// </summary>
	Failed,

	/// <summary>
	/// The command was not recognized or supported.
	/// </summary>
	InvalidCommand
}