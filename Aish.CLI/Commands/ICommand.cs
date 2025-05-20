namespace Aish.CLI.Commands;

/// <summary>
/// Interface for all CLI commands.
/// </summary>
public interface ICommand
{
	string Name { get; }

	bool Execute(string[] args);
}