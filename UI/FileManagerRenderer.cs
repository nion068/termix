using Spectre.Console;
using Spectre.Console.Rendering;
public class FileManagerRenderer(IconProvider iconProvider)
{
    public void Render(string currentPath, List<FileSystemItem> items, int selectedIndex, IRenderable previewContent, int viewOffset)
    {
        Console.Clear();
        var header = CreateHeader(currentPath);
        var body = CreateBody(items, selectedIndex, previewContent, viewOffset);
        var footer = CreateFooter();

        var layout = new Layout("Root")
            .SplitRows(
                new Layout("Header").Update(header).Size(3),
                new Layout("Body").Update(body),
                new Layout("Footer").Update(footer).Size(3)
            );
        AnsiConsole.Write(layout);
    }

    private Panel CreateHeader(string currentPath)
    {
        var displayPath = currentPath.Length > 80 ? "..." + currentPath[^77..] : currentPath;
        var headerContent = new Markup($"[bold cyan3]\uE5FF {displayPath.EscapeMarkup()}[/]");
        return new Panel(headerContent) { Border = BoxBorder.Rounded, BorderStyle = new Style(Color.Cyan1) };
    }

    private Layout CreateBody(List<FileSystemItem> items, int selectedIndex, IRenderable previewContent, int viewOffset)
    {
        var fileTable = CreateFileTable(items, selectedIndex, viewOffset);
        return new Layout("Body").SplitColumns(
            new Layout("FileList").Update(fileTable).Ratio(2),
            new Layout("Preview").Update(previewContent).Ratio(3)
        );
    }

    private Table CreateFileTable(List<FileSystemItem> items, int selectedIndex, int viewOffset)
    {
        var table = new Table().Expand().Border(TableBorder.None);
        table.AddColumn("Name");
        table.AddColumn(new TableColumn("Size").RightAligned());
        table.AddColumn(new TableColumn("Modified").RightAligned());

        table.AddColumn(new TableColumn("").Width(1));

        if (!items.Any())
        {
            table.AddRow(new Markup("[grey]-- Empty --[/]"), new Markup(""), new Markup(""), new Markup(""));
            return table;
        }

        int pageSize = Console.WindowHeight - 12;
        pageSize = Math.Max(5, pageSize);
        var visibleItems = items.Skip(viewOffset).Take(pageSize).ToList();


        bool canScrollUp = viewOffset > 0;
        bool canScrollDown = viewOffset + pageSize < items.Count;
        int thumbPosition = -1;


        if (items.Count > pageSize)
        {

            thumbPosition = (int)Math.Floor((double)selectedIndex / (items.Count - 1) * (visibleItems.Count - 1));
        }

        for (int i = 0; i < visibleItems.Count; i++)
        {
            var item = visibleItems[i];
            var originalIndex = i + viewOffset;
            var isSelected = originalIndex == selectedIndex;
            var style = isSelected ? new Style(background: Color.DodgerBlue1) : Style.Plain;
            var name = CreateNameMarkup(item);


            string scrollChar = " ";
            if (items.Count > pageSize)
            {
                if (i == 0 && canScrollUp) scrollChar = "⬆";
                else if (i == visibleItems.Count - 1 && canScrollDown) scrollChar = "⬇";
                else if (i == thumbPosition) scrollChar = "█";
                else scrollChar = "║";
            }

            var scrollbarMarkup = new Markup($"[grey50]{scrollChar}[/]");
            var row = new IRenderable[]
            {
                new Markup(name, style),
                new Markup(item.FormattedSize, style),
                new Markup(item.FormattedDate, style),
                scrollbarMarkup
            };
            table.AddRow(row);
        }
        return table;
    }

    private string CreateNameMarkup(FileSystemItem item)
    {
        string icon = iconProvider.GetIcon(item);
        string name = item.Name.EscapeMarkup();
        string nameStyle = item.IsDirectory ? "bold" : "";
        return $"{icon}  [{nameStyle}]{name}[/]";
    }

    private Panel CreateFooter()
    {
        var instructions = new Markup(
            "[grey]Use[/] [cyan]↑↓/JK[/] [grey]to move[/] | [cyan]Enter[/] [grey]to open[/] | [cyan]Backspace[/] [grey]for parent[/] | [cyan]Q[/] [grey]to quit[/]"
        );
        return new Panel(Align.Center(instructions)) { Border = BoxBorder.None };
    }

    public void ShowError(string message)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold red]Error:[/] [red]{message.EscapeMarkup()}[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    public Layout GetLayout(string currentPath, List<FileSystemItem> items, int selectedIndex, IRenderable previewContent, int viewOffset)
    {

        var header = CreateHeader(currentPath);
        var body = CreateBody(items, selectedIndex, previewContent, viewOffset);
        var footer = CreateFooter();

        return new Layout("Root")
            .SplitRows(
                new Layout("Header").Update(header).Size(3),
                new Layout("Body").Update(body),
                new Layout("Footer").Update(footer).Size(3)
            );
    }
}