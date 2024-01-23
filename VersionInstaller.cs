using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
  class VersionInstaller
  {
    private bool versionFound = false;

    internal async Task<bool> CheckRuntimeVersion()
    {
      var command = "dotnet --list-runtimes\r\n";
      var dotnetRuntimeInfo = await ExecuteCommand(command);
      Console.WriteLine(dotnetRuntimeInfo);

      if (dotnetRuntimeInfo.Contains("Microsoft.WindowsDesktop.App 8.0.1") && !versionFound)
        versionFound = true;

      return versionFound;
    }

    internal async Task Install()
    {
      var command = "winget install Microsoft.DotNet.DesktopRuntime.8";
      await ExecuteCommand(command);
    }

    private async Task<string> ExecuteCommand(string command)
    {
      try
      {
        var info = new ProcessStartInfo
        {
          Arguments = $"/c {command}",
          CreateNoWindow = true,
          FileName = "cmd.exe",
          UseShellExecute = false,
          RedirectStandardOutput = true,
          RedirectStandardError = true
        };

        using (var process = new Process())
        {
          process.StartInfo = info;
          var outputListRuntimes = new StringBuilder();

          process.OutputDataReceived += (s, eventData) =>
          {
            if (!string.IsNullOrEmpty(eventData.Data))
              outputListRuntimes.AppendLine(eventData.Data);
          };

          process.Start();
          process.BeginOutputReadLine();
          Console.WriteLine(outputListRuntimes);
          await process.WaitForExitAsync(); 
          return outputListRuntimes.ToString();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error executing command: {ex.Message}");
        return string.Empty;
      }
    }
  }

  public static class ProcessExtensions
  {
    public static Task WaitForExitAsync(this Process process)
    {
      var tcs = new TaskCompletionSource<object>();

      process.EnableRaisingEvents = true;
      process.Exited += (sender, args) => tcs.TrySetResult(null);

      return tcs.Task;
    }
  }
}
