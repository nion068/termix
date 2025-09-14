using System.Diagnostics;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace termix.Services;

public static class ClipboardService
{
    public static ActionResponse SetText(string text)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SetTextWindows(text);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                SetTextMac(text);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SetTextLinux(text);
            }
            else
            {
                return new ActionResponse(false, "[red]Clipboard is not supported on this OS.[/]");
            }

            return new ActionResponse(true, $"[green]Yanked path: '{text.EscapeMarkup()}'[/]");
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Failed to copy to clipboard: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    private static void SetTextWindows(string text)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c \"echo " + text.Replace("\"", "\\\"") + " | clip\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        process.WaitForExit();
    }

    private static void SetTextMac(string text)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "pbcopy",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        process.StandardInput.Write(text);
        process.StandardInput.Close();
        process.WaitForExit();
    }

    private static void SetTextLinux(string text)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "xclip",
                Arguments = "-selection clipboard",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        try
        {
            process.Start();
            process.StandardInput.Write(text);
            process.StandardInput.Close();
            process.WaitForExit();
        }
        catch (Exception)
        {
            throw new Exception("xclip is not installed. Please install it to use the clipboard feature.");
        }
    }
}
