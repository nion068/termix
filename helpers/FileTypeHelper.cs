using System.Buffers;
using System.Collections.Frozen;

namespace termix.Helpers;

public static class FileTypeHelper
{
    private static readonly FrozenSet<string> ArchiveExtensions = FrozenSet.ToFrozenSet(
        [".zip", ".rar", ".tar", ".gz", ".7z"],
        StringComparer.OrdinalIgnoreCase);
    private static readonly FrozenSet<string> ImageExtensions = FrozenSet.ToFrozenSet(
        [".jpg", ".jpeg", ".png", ".gif", ".bmp",  ".tif", ".tiff"],
        StringComparer.OrdinalIgnoreCase);

    public static bool IsArchiveFile(string extension)
    {
        return ArchiveExtensions.Contains(extension);
    }
    public static bool IsImageFile(string extension)
    {
        return ImageExtensions.Contains(extension);
    }

    public static bool IsBinary(Stream fileStream)
    {
        if (fileStream.CanSeek)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
        }

        const int sampleSize = 8196; // 8KB
        var buffer = ArrayPool<byte>.Shared.Rent(sampleSize);
        var bytesRead = fileStream.Read(buffer, 0, sampleSize);
        var span = buffer.AsSpan(0, bytesRead);

        return span.Contains((byte)0);
    }
}
