using Aish.Core.Abstractions;

namespace Aish.Core.Services;

/// <summary>
/// Provides a central registry for discovering and managing modules in the shell.
/// </summary>
public class ModuleRegistryService
{
	private readonly Dictionary<string, IModule> _modules = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Registers a new module if it is not already registered.
	/// </summary>
	/// <param name="module">The module to register.</param>
	public void RegisterModule(IModule module)
	{
		if(!_modules.ContainsKey(module.Name))
			_modules[module.Name] = module;
	}

	/// <summary>
	/// Gets a registered module by its name.
	/// </summary>
	/// <param name="name">The name of the module.</param>
	/// <returns>The module instance if found; otherwise <c>null</c>.</returns>
	public IModule? GetModuleByName(string name) =>
		 _modules.TryGetValue(name, out var module) ? module : null;

	/// <summary>
	/// Returns all registered modules.
	/// </summary>
	/// <returns>An enumerable of registered modules.</returns>
	public IEnumerable<IModule> GetAllModules() => _modules.Values;
}