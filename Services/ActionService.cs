using Spectre.Console;
using termix.models;

namespace termix.Services;

public record ActionResponse(bool Success, string Message, object? Payload = null);

public class ActionService
{
    public static ActionResponse Search(string basePath, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new ActionResponse(false, "Search query was empty.");
        }

        var results = new List<FileSystemItem>();
        try
        {
            var ignoreService = new IgnoreService(basePath);
            RecursiveSearch(new DirectoryInfo(basePath), query, ignoreService, basePath, results);

            var message = $"[green]Found {results.Count} results for '{query.EscapeMarkup()}'[/]";
            return new ActionResponse(true, message, results);
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Search failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }
    
    private static void RecursiveSearch(DirectoryInfo currentDir, string query, IgnoreService ignoreService, string rootPath, List<FileSystemItem> results)
    {
        if (ignoreService.IsIgnored(currentDir.FullName))
        {
            return;
        }

        results.AddRange(from file in currentDir.EnumerateFiles() where !ignoreService.IsIgnored(file.FullName) where file.Name.Contains(query, StringComparison.OrdinalIgnoreCase) select new FileSystemItem(file.FullName, Path.GetRelativePath(rootPath, file.FullName), false, file.Length, file.LastWriteTime));

        foreach (var subDir in currentDir.EnumerateDirectories())
        {
            RecursiveSearch(subDir, query, ignoreService, rootPath, results);
        }
    }

    public static ActionResponse Create(string basePath, string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return new ActionResponse(false, "Input was empty.");
        }
        if (userInput.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            return new ActionResponse(false, "[red]Invalid characters in path.[/]");
        }

        var finalPath = userInput;
        var isDir = finalPath.EndsWith(Path.DirectorySeparatorChar) || finalPath.EndsWith(Path.AltDirectorySeparatorChar);
        if (!isDir && !Path.HasExtension(finalPath))
        {
            finalPath += ".txt";
        }

        var newPath = Path.Combine(basePath, finalPath);
        if (File.Exists(newPath) || Directory.Exists(newPath))
        {
            return new ActionResponse(false, $"[red]'{finalPath.EscapeMarkup()}' already exists.[/]");
        }

        try
        {
            if (isDir) Directory.CreateDirectory(newPath);
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newPath)!);
                File.Create(newPath).Close();
            }

            var createdName = Path.GetFileName(newPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            return new ActionResponse(true, $"[green]Created '{createdName.EscapeMarkup()}'[/]", createdName);
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Create failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    public static ActionResponse Rename(string basePath, string oldName, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || newName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return new ActionResponse(false, "[red]Invalid name.[/]");
        }

        var oldPath = Path.Combine(basePath, oldName);
        var newPath = Path.Combine(basePath, newName);

        if (File.Exists(newPath) || Directory.Exists(newPath))
        {
            return new ActionResponse(false, $"[red]'{newName.EscapeMarkup()}' already exists.[/]");
        }
        
        try
        {
            Directory.Move(oldPath, newPath);
            return new ActionResponse(true, $"[green]Renamed to '{newName.EscapeMarkup()}'[/]", newName);
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Rename failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    public static ActionResponse Delete(FileSystemItem item)
    {
        try
        {
            if (item.IsDirectory)
            {
                Directory.Delete(item.Path, true);
            }
            else
            {
                File.Delete(item.Path);
            }
            return new ActionResponse(true, $"[green]Deleted '{item.Name.EscapeMarkup()}'[/]");
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Delete failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }
}
