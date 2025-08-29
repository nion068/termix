using System.Diagnostics;
using Spectre.Console;
using termix.models;
using static termix.FileManager;

namespace termix.Services;

public abstract class FileSystemService
{
    public static List<FileSystemItem> GetDirectoryContents(string path, SortBy sortBy, SortDirection sortDirection,
        bool groupDirectories)
    {
        var items = new List<FileSystemItem>();
        var directoryInfo = new DirectoryInfo(path);

        if (directoryInfo.Parent != null)
            items.Add(new FileSystemItem(
                directoryInfo.Parent.FullName, "..", true, 0,
                directoryInfo.Parent.LastWriteTime, true
            ));

        var directories = directoryInfo.GetDirectories()
            .Select(d => new FileSystemItem(d.FullName, d.Name, true, 0, d.LastWriteTime));

        var files = directoryInfo.GetFiles()
            .Select(f => new FileSystemItem(f.FullName, f.Name, false, f.Length, f.LastWriteTime));

        var allItems = directories.Concat(files);

        var primarySort = groupDirectories
            ? allItems.OrderByDescending(item => item.IsDirectory)
            : allItems.OrderBy(item => 0);

        var sortedItems = sortBy switch
        {
            SortBy.Date => sortDirection == SortDirection.Ascending
                ? primarySort.ThenBy(item => item.LastModified)
                : primarySort.ThenByDescending(item => item.LastModified),
            SortBy.Size => sortDirection == SortDirection.Ascending
                ? primarySort.ThenBy(item => item.Size)
                : primarySort.ThenByDescending(item => item.Size),
            _ => sortDirection == SortDirection.Ascending
                ? primarySort.ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                : primarySort.ThenByDescending(item => item.Name, StringComparer.OrdinalIgnoreCase)
        };

        items.AddRange(sortedItems);

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