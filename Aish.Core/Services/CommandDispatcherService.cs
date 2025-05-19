using Aish.Core.Enums;
using Aish.Core.Models;

namespace Aish.Core.Services;

/// <summary>
/// Dispatches input commands to AISH modules or the system shell depending on context and syntax.
/// </summary>
public class CommandDispatcherService
{
	private readonly ModuleRegistryService _moduleRegistry;
	private readonly IExternalCommandExecutor _externalExecutor;

	/// <summary>
	/// Initializes a new instance of the <see cref="CommandDispatcherService"/>.
	/// </summary>
	/// <param name="moduleRegistry">Module registry for internal command resolution.</param>
	/// <param name="externalExecutor">Executor used to run system shell commands.</param>
	public CommandDispatcherService(ModuleRegistryService moduleRegistry, IExternalCommandExecutor externalExecutor)
	{
		_moduleRegistry = moduleRegistry;
		_externalExecutor = externalExecutor;
	}

	/// <summary>
	/// Processes the command input and routes it to the appropriate AISH handler or host shell.
	/// </summary>
	/// <param name="input">The full command line string.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A <see cref="CommandResult"/> indicating the outcome of execution.</returns>
	public async Task<CommandResult> DispatchAsync(string input, CancellationToken cancellationToken = default)
	{
		if(string.IsNullOrWhiteSpace(input))
			return CommandResult.InvalidCommand;

		var trimmedInput = input.Trim();
		var context = new CommandContext(trimmedInput);

		var prefix = GetPrefix(trimmedInput, out var commandLine);

		var parts = commandLine.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
		if(parts.Length == 0)
			return CommandResult.InvalidCommand;

		var commandKeyword = parts[0];

		switch(prefix)
		{
			case "ash":
				return await ExecuteInternalOnlyAsync(commandKeyword, context, cancellationToken);

			case "sh":
				{
					var external = await _externalExecutor.ExecuteExternalAsync(commandLine, cancellationToken);
					if(external == CommandResult.Success)
						return external;
					return await ExecuteInternalOnlyAsync(commandKeyword, context, cancellationToken);
				}

			default:
				{
					var internalResult = await ExecuteInternalOnlyAsync(commandKeyword, context, cancellationToken);
					if(internalResult == CommandResult.Success)
						return internalResult;
					return await _externalExecutor.ExecuteExternalAsync(commandLine, cancellationToken);
				}
		}
	}

	private string GetPrefix(string input, out string commandWithoutPrefix)
	{
		if(input.Contains('.'))
		{
			var split = input.Split('.', 2);
			var prefix = split[0].ToLowerInvariant();
			commandWithoutPrefix = split[1].Trim();
			return prefix;
		}

		commandWithoutPrefix = input;
		return string.Empty;
	}

	private async Task<CommandResult> ExecuteInternalOnlyAsync(string commandKeyword, CommandContext context, CancellationToken cancellationToken)
	{
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