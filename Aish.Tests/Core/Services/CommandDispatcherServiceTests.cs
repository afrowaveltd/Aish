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
	private readonly CommandDispatcherService _sut;

	public CommandDispatcherServiceTests()
	{
		_moduleRegistry = new ModuleRegistryService();
		_sut = new CommandDispatcherService(_moduleRegistry);
	}

	[Fact]
	public async Task DispatchAsync_Should_Invoke_Command_Handler_If_Found()
	{
		var handler = Substitute.For<ICommandHandler>();
		handler.Command.Returns("greet");
		handler.ExecuteAsync(Arg.Any<CommandContext>(), Arg.Any<CancellationToken>())
			 .Returns(Task.FromResult(CommandResult.Success));

		var module = Substitute.For<IModule>();
		module.Name.Returns("GreetingModule");
		module.GetCommands().Returns(new[] { handler });

		_moduleRegistry.RegisterModule(module);

		var result = await _sut.DispatchAsync("greet");

		result.Should().Be(CommandResult.Success);
		await handler.Received(1).ExecuteAsync(Arg.Any<CommandContext>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task DispatchAsync_Should_Return_InvalidCommand_If_Handler_Not_Found()
	{
		var module = Substitute.For<IModule>();
		module.Name.Returns("EmptyModule");
		module.GetCommands().Returns(Enumerable.Empty<ICommandHandler>());

		_moduleRegistry.RegisterModule(module);

		var result = await _sut.DispatchAsync("unknown");

		result.Should().Be(CommandResult.InvalidCommand);
	}

	[Fact]
	public async Task DispatchAsync_Should_Return_InvalidCommand_If_Input_Is_Empty()
	{
		var result = await _sut.DispatchAsync("");

		result.Should().Be(CommandResult.InvalidCommand);
	}
}