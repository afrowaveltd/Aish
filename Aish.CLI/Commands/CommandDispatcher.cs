namespace Aish.CLI.Commands;

/// <summary>
/// Finds and executes a matching command based on user input.
/// </summary>
public class CommandDispatcher(IEnumerable<ICommand> commands)
{
	private readonly IEnumerable<ICommand> _commands = commands;

	public bool Dispatch(string input)
	{
		var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if(parts.Length == 0) return true;

		var command = _commands.FirstOrDefault(c => c.Name.Equals(parts[0], StringComparison.OrdinalIgnoreCase));
		if(command is null)
		{
			Console.WriteLine("❌ Unknown command.");
			return true;
		}

		return command.Execute([.. parts.Skip(1)]);
	}
}