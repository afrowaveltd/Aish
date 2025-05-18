namespace Aish.Core.Abstractions;

/// <summary>
/// Represents a functional module that provides a set of commands to the shell.
/// </summary>
public interface IModule
{
	/// <summary>
	/// Gets the unique name of the module.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Gets a short description of what the module provides.
	/// </summary>
	string Description { get; }

	/// <summary>
	/// Gets all commands available within this module.
	/// </summary>
	/// <returns>A collection of <see cref="ICommandHandler"/> provided by the module.</returns>
	IEnumerable<ICommandHandler> GetCommands();
}