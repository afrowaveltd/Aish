using Aish.Core.Abstractions;
using Aish.Core.Enums;
using Aish.Core.Models;
using Aish.Core.Services;
using FluentAssertions;
using NSubstitute;

namespace Aish.Tests.Core.Services;

public class CommandDispatcherServiceTests
{
	private readonly ModuleRegistryService _moduleRegistry;
	private readonly IExternalCommandExecutor _externalExecutor;
	private readonly CommandDispatcherService _sut;

	public CommandDispatcherServiceTests()
	{
		_moduleRegistry = new ModuleRegistryService();
		_externalExecutor = Substitute.For<IExternalCommandExecutor>();

		// Default: all external calls return Failed unless configured otherwise
		_externalExecutor.ExecuteExternalAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			 .Returns(Task.FromResult(CommandResult.Failed));

		_sut = new CommandDispatcherService(_moduleRegistry, _externalExecutor);
	}

	[Fact]
	public async Task DispatchAsync_Should_Invoke_Command_Handler_If_Found()
	{
		var handler = Substitute.For<ICommandHandler>();
		handler.Command.Returns("greet");
		handler.ExecuteAsync(Arg.Any<CommandContext>(), Arg.Any<CancellationToken>())
			 .Returns(Task.FromResult(CommandResult.Success));

		var module = Substitute.For<IModule>();
		module.Name.Returns("TestModule");
		module.GetCommands().Returns(new[] { handler });

		_moduleRegistry.RegisterModule(module);

		var result = await _sut.DispatchAsync("greet");

		result.Should().Be(CommandResult.Success);
		await handler.Received(1).ExecuteAsync(Arg.Any<CommandContext>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task DispatchAsync_Should_Fallback_To_External_If_Handler_Not_Found()
	{
		var result = await _sut.DispatchAsync("unknown-command");
		result.Should().Be(CommandResult.Failed); // Because mocked executor returns Failed
		await _externalExecutor.Received(1).ExecuteExternalAsync("unknown-command", Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task DispatchAsync_Should_Return_InvalidCommand_For_Empty_Input()
	{
		var result = await _sut.DispatchAsync("");
		result.Should().Be(CommandResult.InvalidCommand);
	}

	[Fact]
	public async Task DispatchAsync_Should_Execute_External_Then_Internal_For_sh_Prefix()
	{
		var handler = Substitute.For<ICommandHandler>();
		handler.Command.Returns("greet");
		handler.ExecuteAsync(Arg.Any<CommandContext>(), Arg.Any<CancellationToken>())
			 .Returns(Task.FromResult(CommandResult.Success));

		var module = Substitute.For<IModule>();
		module.Name.Returns("TestModule");
		module.GetCommands().Returns(new[] { handler });
		_moduleRegistry.RegisterModule(module);

		// Simulate system failure to fall back on AISH
		_externalExecutor.ExecuteExternalAsync("greet", Arg.Any<CancellationToken>())
			 .Returns(Task.FromResult(CommandResult.Failed));

		var result = await _sut.DispatchAsync("sh.greet");

		result.Should().Be(CommandResult.Success);
		await _externalExecutor.Received(1).ExecuteExternalAsync("greet", Arg.Any<CancellationToken>());
		await handler.Received(1).ExecuteAsync(Arg.Any<CommandContext>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task DispatchAsync_Should_Only_Execute_Internal_For_ash_Prefix()
	{
		var handler = Substitute.For<ICommandHandler>();
		handler.Command.Returns("whoami");
		handler.ExecuteAsync(Arg.Any<CommandContext>(), Arg.Any<CancellationToken>())
			 .Returns(Task.FromResult(CommandResult.Success));

		var module = Substitute.For<IModule>();
		module.Name.Returns("InternalModule");
		module.GetCommands().Returns(new[] { handler });
		_moduleRegistry.RegisterModule(module);

		var result = await _sut.DispatchAsync("ash.whoami");

		result.Should().Be(CommandResult.Success);
		await handler.Received(1).ExecuteAsync(Arg.Any<CommandContext>(), Arg.Any<CancellationToken>());
		await _externalExecutor.Received(0).ExecuteExternalAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
	}
}