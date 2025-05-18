using Aish.Core.Enums;
using Aish.Core.Models;

namespace Aish.Core.Abstractions;

/// <summary>
/// Defines a command that can be executed in the AISH shell.
/// </summary>
public interface ICommandHandler
{
	/// <summary>
	/// Gets the unique keyword used to trigger this command.
	/// </summary>
	string Command { get; }

	/// <summary>
	/// Gets a short description of the command's purpose.
	/// </summary>
	string Description { get; }

	/// <summary>
	/// Executes the command using the given context.
	/// </summary>
	/// <param name="context">The execution context for the command.</param>
	/// <param name="cancellationToken">Optional cancellation token.</param>
	/// <returns>A task that completes with the result of the command execution.</returns>
	Task<CommandResult> ExecuteAsync(CommandContext context, CancellationToken cancellationToken = default);
}