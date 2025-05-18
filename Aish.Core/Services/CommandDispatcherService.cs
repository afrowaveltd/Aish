using Aish.Core.Enums;

namespace Aish.Core.Services;

/// <summary>
/// Dispatches user input to the appropriate module and command handler.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CommandDispatcherService"/>.
/// </remarks>
/// <param name="moduleRegistry">The registry used to resolve modules and commands.</param>
public class CommandDispatcherService(ModuleRegistryService moduleRegistry)
{
	private readonly ModuleRegistryService _moduleRegistry = moduleRegistry;

	/// <summary>
	/// Processes the given input and attempts to find and execute a matching command.
	/// </summary>
	/// <param name="input">The raw user input.</param>
	/// <param name="cancellationToken">Optional cancellation token.</param>
	/// <returns>A <see cref="CommandResult"/> representing the outcome of the execution.</returns>
	public async Task<CommandResult> DispatchAsync(string input, CancellationToken cancellationToken = default)
	{
		if(string.IsNullOrWhiteSpace(input))
			return CommandResult.InvalidCommand;

		var parts = input.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
		if(parts.Length == 0)
			return CommandResult.InvalidCommand;

		var commandKeyword = parts[0];
		var context = new Aish.Core.Models.CommandContext(input);

		foreach(var module in _moduleRegistry.GetAllModules())
		{
			var handler = module.GetCommands().FirstOrDefault(c =>
				 string.Equals(c.Command, commandKeyword, StringComparison.OrdinalIgnoreCase));

			if(handler is not null)
				return await handler.ExecuteAsync(context, cancellationToken);
		}

		return CommandResult.InvalidCommand;
	}
}