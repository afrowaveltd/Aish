using Aish.Core.Enums;
using System.Diagnostics;

namespace Aish.Core.Services;

/// <summary>
/// Executes raw command strings in the host operating system's default shell environment.
/// </summary>
public class ExternalCommandExecutor : IExternalCommandExecutor
{
	/// <summary>
	/// Runs the given input command through the underlying shell (cmd, bash, etc.).
	/// </summary>
	/// <param name="input">The full command line to execute.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A <see cref="CommandResult"/> based on process outcome.</returns>
	public async Task<CommandResult> ExecuteExternalAsync(string input, CancellationToken cancellationToken = default)
	{
		try
		{
			using var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = GetShellExecutable(),
					Arguments = GetShellArguments(input),
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();

			var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
			var error = await process.StandardError.ReadToEndAsync(cancellationToken);

			await process.WaitForExitAsync(cancellationToken);

			if(!string.IsNullOrWhiteSpace(output))
				Console.WriteLine(output.Trim());

			if(!string.IsNullOrWhiteSpace(error))
				Console.Error.WriteLine(error.Trim());

			return process.ExitCode == 0 ? CommandResult.Success : CommandResult.Failed;
		}
		catch(Exception ex)
		{
			Console.Error.WriteLine($"[system error] {ex.Message}");
			return CommandResult.Failed;
		}
	}

	/// <summary>
	/// Determines the executable to use for system shell commands.
	/// </summary>
	private string GetShellExecutable()
	{
		if(OperatingSystem.IsWindows()) return "cmd.exe";
		if(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) return "/bin/bash";
		throw new PlatformNotSupportedException("Unsupported platform for shell execution.");
	}

	/// <summary>
	/// Prepares arguments for shell execution depending on OS.
	/// </summary>
	/// <param name="input">The user input to pass to the shell.</param>
	private string GetShellArguments(string input)
	{
		return OperatingSystem.IsWindows() ? $"/c {input}" : $"-c \"{input}\"";
	}
}