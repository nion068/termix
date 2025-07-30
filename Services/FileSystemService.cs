using System.Diagnostics;
using Spectre.Console;
using termix.models;

namespace termix.Services;

public abstract class FileSystemService
{
    public static List<FileSystemItem> GetDirectoryContents(string path)
    {
        var items = new List<FileSystemItem>();
        var directoryInfo = new DirectoryInfo(path);

        if (directoryInfo.Parent != null)
        {
            items.Add(new FileSystemItem(
                directoryInfo.Parent.FullName, "..", true, 0,
                directoryInfo.Parent.LastWriteTime, IsParentDirectory: true
            ));
        }

        var directories = directoryInfo.GetDirectories()
            .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
            .Select(d => new FileSystemItem(d.FullName, d.Name, true, 0, d.LastWriteTime));
        items.AddRange(directories);

        var files = directoryInfo.GetFiles()
            .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
            .Select(f => new FileSystemItem(f.FullName, f.Name, false, f.Length, f.LastWriteTime));
        items.AddRange(files);

        return items;
    }

    public static void OpenFile(string filePath)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true };
            Process.Start(processStartInfo);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error opening file: {ex.Message}[/]");
            Console.ReadKey(true);
        }
    }
}
