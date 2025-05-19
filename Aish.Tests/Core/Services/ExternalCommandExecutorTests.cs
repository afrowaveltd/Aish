using Aish.Core.Enums;
using Aish.Core.Services;
using FluentAssertions;

namespace Aish.Tests.Core.Services;

public class ExternalCommandExecutorTests
{
	private readonly ExternalCommandExecutor _executor = new();

	[Fact]
	public async Task ExecuteExternalAsync_Should_Return_Success_For_Valid_Universal_Command()
	{
		// `cd` is universally available and should not fail even without arguments
		var result = await _executor.ExecuteExternalAsync("cd");

		result.Should().Be(CommandResult.Success);
	}

	[Fact]
	public async Task ExecuteExternalAsync_Should_Return_Failed_For_Invalid_Command()
	{
		var result = await _executor.ExecuteExternalAsync("nonexistent-xyz123456");

		result.Should().Be(CommandResult.Failed);
	}
}