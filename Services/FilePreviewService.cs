using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace termix.Services;

public class FilePreviewService
{
    private static readonly string[] SourceArray = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];
    private readonly CustomSyntaxHighlighter _highlighter = new();
    private const string TabReplacement = "    ";

    public IRenderable GetPreview(string? filePath, int verticalOffset, int horizontalOffset)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return new Panel(Align.Center(new Text("Select a file to preview"), VerticalAlignment.Middle))
                .Expand().Border(BoxBorder.Rounded).Header("[grey]Preview[/]");

        var fileInfo = new FileInfo(filePath);
        var header = $"[white]Preview (Scroll: Alt+Arrows):[/] [aqua]{fileInfo.Name.EscapeMarkup()}[/]";

        try
        {
            if (IsImageFile(fileInfo.Extension))
                return new Panel(Align.Center(new Text("Image File\nPreview not supported yet."),
                        VerticalAlignment.Middle))
                    .Header(header).Expand().Border(BoxBorder.Rounded);

            var fileBytes = File.ReadAllBytes(filePath);
            if (IsBinary(fileBytes))
                return new Panel(Align.Center(new Text("Binary File\nNo preview available"), VerticalAlignment.Middle))
                    .Header(header).Expand().Border(BoxBorder.Rounded);

            var previewHeight = Console.WindowHeight - 12;
            var allLines = File.ReadAllLines(filePath, Encoding.UTF8);

            var visibleLines = allLines
                .Skip(verticalOffset)
                .Take(previewHeight)
                .Select(line => line.Length > horizontalOffset ? line[horizontalOffset..].Replace("\t", TabReplacement) : "")
                .ToArray();

            IRenderable content;

            if (visibleLines.Length == 0)
                content = Align.Center(new Text("[grey]-- End of File --[/]"), VerticalAlignment.Middle);
            else
                content = _highlighter.Highlight(string.Join("\n", visibleLines), fileInfo.Extension);

            return new Panel(content).Header(header).Expand().Border(BoxBorder.Rounded);
        }
        catch (IOException ex)
        {
            return new Panel(Align.Center(new Text($"[red]Error reading file:[/] {ex.Message.EscapeMarkup()}"),
                    VerticalAlignment.Middle))
                .Header(header).Expand().Border(BoxBorder.Rounded);
        }
    }

    private static bool IsImageFile(string extension)
    {
        return SourceArray.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsBinary(byte[] fileBytes)
    {
        return fileBytes.Take(8000).Any(b => b == 0);
    }
}