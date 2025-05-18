using Aish.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aish.Core.Extensions;

/// <summary>
/// Provides extension methods to register AISH core services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers all core services required for AISH operation.
	/// </summary>
	/// <param name="services">The service collection to configure.</param>
	/// <returns>The updated service collection.</returns>
	public static IServiceCollection AddAishCore(this IServiceCollection services)
	{
		services.AddSingleton<ModuleRegistryService>();
		services.AddSingleton<CommandDispatcherService>();

		return services;
	}
}