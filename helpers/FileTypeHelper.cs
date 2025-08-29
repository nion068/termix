namespace termix.Helpers;

public static class FileTypeHelper
{
    private static readonly string[] ArchiveExtensions = [".zip", ".rar", ".tar", ".gz", ".7z"];
    private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];

    public static bool IsArchiveFile(string extension)
    {
        return ArchiveExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
    public static bool IsImageFile(string extension)
    {
        return ImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsBinary(byte[] fileBytes)
    {
        return fileBytes.Take(8000).Any(b => b == 0);
    }
}
