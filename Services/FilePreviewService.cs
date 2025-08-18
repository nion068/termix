using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using termix.models;

namespace termix.Services;

public class FilePreviewService(IconProvider iconProvider)
{
    private static readonly string[] SourceArray = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];
    private readonly CustomSyntaxHighlighter _highlighter = new();
    private const string TabReplacement = "    ";

    public IRenderable GetPreview(string? filePath, int verticalOffset, int horizontalOffset)
    {
        string header;

        if (string.IsNullOrEmpty(filePath) || (!File.Exists(filePath) && !Directory.Exists(filePath)))
            return new Panel(Align.Center(new Text("Select a file to preview"), VerticalAlignment.Middle))
                .Expand().Border(BoxBorder.Rounded).Header("[grey]Preview[/]");

        if (Directory.Exists(filePath))
        {
            var directoryInfo = new DirectoryInfo(filePath);
            header = $"[white]Preview (Directory):[/] [aqua]{directoryInfo.Name.EscapeMarkup()}[/]";
            return RenderDirectoryPreview(filePath, header);
        }

        var fileInfo = new FileInfo(filePath);
        header = $"[white]Preview (Scroll: Alt+Arrows):[/] [aqua]{fileInfo.Name.EscapeMarkup()}[/]";

        try
        {
            if (IsImageFile(fileInfo.Extension))
            {
                return RenderImageWithMaximumRefinement(filePath, header);
            }

            var fileBytes = File.ReadAllBytes(filePath);
            if (IsBinary(fileBytes))
                return new Panel(Align.Center(new Text("Binary File\nNo preview available"), VerticalAlignment.Middle))
                    .Header(header).Expand().Border(BoxBorder.Rounded);

            var textPreviewHeight = Console.WindowHeight - 12;
            var allLines = File.ReadAllLines(filePath, Encoding.UTF8);

            var visibleLines = allLines
                .Skip(verticalOffset)
                .Take(textPreviewHeight)
                .Select(line =>
                    line.Length > horizontalOffset ? line[horizontalOffset..].Replace("\t", TabReplacement) : "")
                .ToArray();

            IRenderable content = visibleLines.Length == 0
                ? Align.Center(new Text("[grey]-- End of File --[/]"), VerticalAlignment.Middle)
                : _highlighter.Highlight(string.Join("\n", visibleLines), fileInfo.Extension);

            return new Panel(content).Header(header).Expand().Border(BoxBorder.Rounded);
        }
        catch (Exception ex)
        {
            var errorMessage = ex is UnknownImageFormatException
                ? "[red]Cannot render image: Unsupported or corrupt file.[/]"
                : $"[red]Error reading file:[/] {ex.Message.EscapeMarkup()}";

            return new Panel(Align.Center(new Text(errorMessage), VerticalAlignment.Middle))
                .Header(header).Expand().Border(BoxBorder.Rounded);
        }
    }

    private static IRenderable RenderImageWithMaximumRefinement(string filePath, string header)
    {
        using var image = Image.Load<Rgba32>(filePath);

        const int horizontalFactor = 3;
        const int verticalFactor = 3;

        var consoleWidth = (Console.WindowWidth / 2) - 10;
        var consoleHeight = Console.WindowHeight - 12;

        var targetWidth = consoleWidth * horizontalFactor;
        var targetHeight = consoleHeight * 2 * verticalFactor;

        image.Mutate(ctx =>
        {
            ctx.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new SixLabors.ImageSharp.Size(targetWidth, targetHeight),
                Sampler = KnownResamplers.Lanczos3
            });
            ctx.Dither(KnownDitherings.FloydSteinberg);
        });

        var sb = new StringBuilder();
        const int sampleArea = horizontalFactor * verticalFactor;

        for (var y = 0; y < image.Height - (verticalFactor * 2 - 1); y += verticalFactor * 2)
        {
            for (var x = 0; x < image.Width - (horizontalFactor - 1); x += horizontalFactor)
            {
                long topR = 0, topG = 0, topB = 0;
                long botR = 0, botG = 0, botB = 0;

                for (var offsetY = 0; offsetY < verticalFactor; offsetY++)
                {
                    for (var offsetX = 0; offsetX < horizontalFactor; offsetX++)
                    {
                        var pTop = image[x + offsetX, y + offsetY];
                        topR += pTop.R;
                        topG += pTop.G;
                        topB += pTop.B;

                        var pBot = image[x + offsetX, y + offsetY + verticalFactor];
                        botR += pBot.R;
                        botG += pBot.G;
                        botB += pBot.B;
                    }
                }

                var upperColor = new Rgba32((byte)(topR / sampleArea), (byte)(topG / sampleArea),
                    (byte)(topB / sampleArea));
                var lowerColor = new Rgba32((byte)(botR / sampleArea), (byte)(botG / sampleArea),
                    (byte)(botB / sampleArea));

                sb.Append(
                    $"[rgb({lowerColor.R},{lowerColor.G},{lowerColor.B}) on rgb({upperColor.R},{upperColor.G},{upperColor.B})]▄[/]");
            }

            sb.AppendLine();
        }

        return new Panel(Align.Center(new Markup(sb.ToString()), VerticalAlignment.Middle))
            .Header(header)
            .Expand()
            .Border(BoxBorder.Rounded);
    }

    private IRenderable RenderDirectoryPreview(string directoryPath, string header)
    {
        try
        {
            var items = FileSystemService
                .GetDirectoryContents(directoryPath, SortBy.Name, SortDirection.Ascending, true)
                .Where(item => !item.IsParentDirectory)
                .Take(Console.WindowHeight - 14)
                .ToList();

            if (items.Count == 0)
            {
                return new Panel(Align.Center(new Text("[grey]-- Empty Directory --[/]"), VerticalAlignment.Middle))
                    .Header(header).Expand().Border(BoxBorder.Rounded);
            }

            var tree = new Tree($"[aqua bold]{new DirectoryInfo(directoryPath).Name.EscapeMarkup()}[/]");
            foreach (var nodeText in from item in items
                     let icon = iconProvider.GetIcon(item)
                     select $"{icon} {item.Name.EscapeMarkup()}")
            {
                tree.AddNode(nodeText);
            }

            return new Panel(tree).Header(header).Expand().Border(BoxBorder.Rounded);
        }
        catch (Exception ex)
        {
            var errorMessage = $"[red]Error reading directory:[/] {ex.Message.EscapeMarkup()}";
            return new Panel(Align.Center(new Text(errorMessage), VerticalAlignment.Middle))
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