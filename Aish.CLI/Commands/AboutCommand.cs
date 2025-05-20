using System.Windows.Input;

namespace Aish.CLI.Commands;

public class AboutCommand : ICommand
{
	public string Name => "about";

	public bool Execute(string[] args)
	{
		var text = "Welcome to AISH – Afrowave Intelligent Shell.\nModular, smart, and multilingual.";
		TypewriterEffect(text);
		return true;
	}

	private void TypewriterEffect(string text)
	{
		foreach(char c in text)
		{
			Console.Write(c);
			Thread.Sleep(20); // pro efekt psacího stroje
		}
		Console.WriteLine();
	}
}