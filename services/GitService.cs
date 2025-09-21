namespace termix.Services;

public static class GitService
{
    public static string? GetBranchName(string path)
    {
        try
        {
            var repoPath = FindGitRepository(path);
            if (repoPath == null)
            {
                return null;
            }

            var headFile = Path.Combine(repoPath, ".git", "HEAD");
            if (!File.Exists(headFile))
            {
                return null;
            }
            
            var headContent = File.ReadAllText(headFile).Trim();

            const string refPrefix = "ref: refs/heads/";
            if (headContent.StartsWith(refPrefix))
            {
                return headContent[refPrefix.Length..];
            }

            return headContent.Length > 7 ? headContent[..7] : headContent;
        }
        catch
        {
            return null;
        }
    }

    private static string? FindGitRepository(string startPath)
    {
        var currentDirectory = new DirectoryInfo(startPath);
        while (currentDirectory != null)
        {
            if (Directory.Exists(Path.Combine(currentDirectory.FullName, ".git")))
            {
                return currentDirectory.FullName;
            }
            currentDirectory = currentDirectory.Parent;
        }
        return null;
    }
}
