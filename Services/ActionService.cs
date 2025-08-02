using Spectre.Console;
using termix.models;

namespace termix.Services;

public record ActionResponse(bool Success, string Message, object? Payload = null);

public abstract class ActionService
{
    public static Task<List<FileSystemItem>> GetDeepDirectoryContentsAsync(string basePath, CancellationToken token)
    {
        return Task.Run(() =>
        {
            var results = new List<FileSystemItem>();
            var ignoreService = new IgnoreService(basePath);
            RecursiveSearch(new DirectoryInfo(basePath), ignoreService, basePath, results, token);
            return results;
        }, token);
    }

    private static void RecursiveSearch(DirectoryInfo currentDir, IgnoreService ignoreService, string rootPath,
        List<FileSystemItem> results, CancellationToken token)
    {
        if (token.IsCancellationRequested || ignoreService.IsIgnored(currentDir.FullName)) return;

        foreach (var entry in currentDir.EnumerateFileSystemInfos())
        {
            if (token.IsCancellationRequested) return;

            switch (entry)
            {
                case DirectoryInfo subDir:
                    RecursiveSearch(subDir, ignoreService, rootPath, results, token);
                    break;
                case FileInfo file when ignoreService.IsIgnored(file.FullName):
                    continue;
                case FileInfo file:
                    results.Add(new FileSystemItem(
                        file.FullName,
                        Path.GetRelativePath(rootPath, file.FullName),
                        false,
                        file.Length,
                        file.LastWriteTime
                    ));
                    break;
            }
        }
    }

    public static ActionResponse Create(string basePath, string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput)) return new ActionResponse(false, "Input was empty.");
        if (userInput.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            return new ActionResponse(false, "[red]Invalid characters in path.[/]");

        var finalPath = userInput;
        var isDir = finalPath.EndsWith(Path.DirectorySeparatorChar) ||
                    finalPath.EndsWith(Path.AltDirectorySeparatorChar);
        if (!isDir && !Path.HasExtension(finalPath)) finalPath += ".txt";

        var newPath = Path.Combine(basePath, finalPath);
        if (File.Exists(newPath) || Directory.Exists(newPath))
            return new ActionResponse(false, $"[red]'{finalPath.EscapeMarkup()}' already exists.[/]");

        try
        {
            if (isDir)
            {
                Directory.CreateDirectory(newPath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newPath)!);
                File.Create(newPath).Close();
            }

            var createdName =
                Path.GetFileName(newPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
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
            return new ActionResponse(false, "[red]Invalid name.[/]");

        var oldPath = Path.Combine(basePath, oldName);
        var newPath = Path.Combine(basePath, newName);

        if (File.Exists(newPath) || Directory.Exists(newPath))
            return new ActionResponse(false, $"[red]'{newName.EscapeMarkup()}' already exists.[/]");

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
                Directory.Delete(item.Path, true);
            else
                File.Delete(item.Path);
            return new ActionResponse(true, $"[green]Deleted '{item.Name.EscapeMarkup()}'[/]");
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Delete failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }
}