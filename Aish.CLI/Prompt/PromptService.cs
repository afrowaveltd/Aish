namespace Aish.CLI.Prompt;

/// <summary>
/// Handles user input and displays the custom prompt with emoji support.
/// </summary>
public class PromptService
{
	private readonly string _promptSymbol = "💡 AISH > ";

	public string ReadInput()
	{
		Console.Write(_promptSymbol);
		return Console.ReadLine()?.Trim() ?? string.Empty;
	}
}