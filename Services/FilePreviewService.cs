using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace termix.Services;

public class FilePreviewService
{
    private readonly CustomSyntaxHighlighter _highlighter = new();

    public IRenderable GetPreview(string? filePath, int verticalOffset, int horizontalOffset)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            return new Panel(Align.Center(new Text("Select a file to preview"), VerticalAlignment.Middle))
                .Expand().Border(BoxBorder.Rounded).Header("[grey]Preview[/]");
        }

        var fileInfo = new FileInfo(filePath);
        var extension = fileInfo.Extension.ToLowerInvariant();
        IRenderable content;
        var header = $"[white]Preview (Scroll: Alt+↑↓/JK, Alt+←→/HL):[/] [aqua]{fileInfo.Name.EscapeMarkup()}[/]";

        try
        {
            var fileBytes = File.ReadAllBytes(filePath);
            if (IsBinary(fileBytes))
            {
                content = Align.Center(new Text("Binary File\nNo preview available"), VerticalAlignment.Middle);
                return new Panel(content).Header(header).Expand().Border(BoxBorder.Rounded);
            }

            var previewHeight = Console.WindowHeight - 12;
            var allLines = File.ReadAllLines(filePath, Encoding.UTF8);

            var visibleLines = allLines
                .Skip(verticalOffset)
                .Take(previewHeight)
                .Select(line => line.Length > horizontalOffset ? line.Substring(horizontalOffset) : "")
                .ToArray();

            if (visibleLines.Length == 0)
            {
                content = Align.Center(new Text("[grey]-- End of File --[/]"), VerticalAlignment.Middle);
            }
            else
            {
                var processedContent = string.Join("\n", visibleLines);

                content = extension switch
                {
                    ".cs" => _highlighter.Highlight(processedContent, "csharp"),
                    ".py" => _highlighter.Highlight(processedContent, "python"),
                    ".js" => _highlighter.Highlight(processedContent, "javascript"), 
                    _ => new Text(processedContent.EscapeMarkup())
                };
            }
        }
        catch (IOException ex)
        {
            content = Align.Center(new Text($"[red]Error reading file:[/] {ex.Message.EscapeMarkup()}"), VerticalAlignment.Middle);
        }

        return new Panel(content).Header(header).Expand().Border(BoxBorder.Rounded);
    }

    private static bool IsBinary(byte[] fileBytes)
    {
        const int sampleSize = 8000;
        var length = Math.Min(sampleSize, fileBytes.Length);

        for (var i = 0; i < length; i++)
        {
            var b = fileBytes[i];
            switch (b)
            {
                case 0:
                case < 7:
                case > 14 and < 32:
                    return true;
            }
        }

        return false;
    }
}