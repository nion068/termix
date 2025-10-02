using System.Text.Json;
using termix.models;

namespace termix.Services;

public class BookmarkService
{
    private readonly string _bookmarksFilePath;

    public BookmarkService()
    {
        var configDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "termix"
        );
        Directory.CreateDirectory(configDir);
        _bookmarksFilePath = Path.Combine(configDir, "bookmarks.json");
    }

    public List<Bookmark> LoadBookmarks()
    {
        if (!File.Exists(_bookmarksFilePath))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(_bookmarksFilePath);
            return JsonSerializer.Deserialize(json, BookmarkJsonContext.Default.ListBookmark) ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public void SaveBookmarks(IEnumerable<Bookmark> bookmarks)
    {
        var json = JsonSerializer.Serialize(bookmarks, BookmarkJsonContext.Default.ListBookmark);
        File.WriteAllText(_bookmarksFilePath, json);
    }
}