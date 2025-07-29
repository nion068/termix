using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text;

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
            byte[] fileBytes = File.ReadAllBytes(filePath);
            if (IsBinary(fileBytes))
            {
                content = Align.Center(new Text("Binary File\nNo preview available"), VerticalAlignment.Middle);
                return new Panel(content).Header(header).Expand().Border(BoxBorder.Rounded);
            }

            int previewHeight = Console.WindowHeight - 12;
            string[] allLines = File.ReadAllLines(filePath, Encoding.UTF8);

            var visibleLines = allLines
                .Skip(verticalOffset)
                .Take(previewHeight)
                .Select(line => line.Length > horizontalOffset ? line.Substring(horizontalOffset) : "")
                .ToArray();

            if (!visibleLines.Any())
            {
                content = Align.Center(new Text("[grey]-- End of File --[/]"), VerticalAlignment.Middle);
            }
            else
            {
                var processedContent = string.Join("\n", visibleLines);

                switch (extension)
                {
                    case ".cs": content = _highlighter.Highlight(processedContent, "csharp"); break;
                    case ".py": content = _highlighter.Highlight(processedContent, "python"); break;
                    case ".js": content = _highlighter.Highlight(processedContent, "javascript"); break;
                    case ".md":
                    case ".txt":
                    case ".log":
                    case ".json":
                    case ".xml":
                    case ".html":
                    case ".css":
                        content = new Text(processedContent.EscapeMarkup());
                        break;
                    default:

                        content = new Text(processedContent.EscapeMarkup());
                        break;
                }
            }
        }
        catch (IOException ex)
        {
            content = Align.Center(new Text($"[red]Error reading file:[/] {ex.Message.EscapeMarkup()}"), VerticalAlignment.Middle);
        }

        return new Panel(content).Header(header).Expand().Border(BoxBorder.Rounded);
    }

    private bool IsBinary(byte[] fileBytes)
    {
        const int sampleSize = 8000;
        int length = Math.Min(sampleSize, fileBytes.Length);

        for (int i = 0; i < length; i++)
        {
            byte b = fileBytes[i];
            if (b == 0) return true;
            if (b < 7 || (b > 14 && b < 32)) return true;
        }

        return false;
    }
}
