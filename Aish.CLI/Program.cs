using Aish.CLI.Commands;
using Aish.CLI.Prompt;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
	 .AddSingleton<PromptService>()
	 .AddSingleton<CommandDispatcher>()
	 .AddSingleton<ICommand, AboutCommand>()
	 // Add more commands here
	 .BuildServiceProvider();

var prompt = services.GetRequiredService<PromptService>();
var dispatcher = services.GetRequiredService<CommandDispatcher>();

while(true)
{
	var input = prompt.ReadInput(); // zobrazí prompt a získá vstup
	if(!dispatcher.Dispatch(input))
		break; // například pro "exit"
}