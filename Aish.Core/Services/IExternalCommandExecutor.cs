using Aish.Core.Enums;

namespace Aish.Core.Services
{
	public interface IExternalCommandExecutor
	{
		Task<CommandResult> ExecuteExternalAsync(string input, CancellationToken cancellationToken = default);
	}
}