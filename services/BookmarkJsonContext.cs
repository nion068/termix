using System.Text.Json.Serialization;
using termix.models;

namespace termix.Services;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Bookmark>))]
public partial class BookmarkJsonContext : JsonSerializerContext
{
}
