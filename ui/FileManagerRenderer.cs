using Spectre.Console;
using Spectre.Console.Rendering;
using termix.models;
using termix.Services;

namespace termix.UI;

public class FileManagerRenderer(IconProvider iconProvider)
{
    public Layout GetLayout(FileManager fm, IRenderable footerRenderable)
    {
        var header = CreateHeader(fm.State.CurrentPath);
        IRenderable body = fm.State.CurrentMode switch
        {
            InputMode.SortMenu => CreateSortMenuBody(fm.SortOptions, fm.State.SortMenuSelectedIndex),
            InputMode.HelpScreen => CreateHelpScreen(fm.State),
            _ => CreateFileBrowserBody(fm.State, fm.State.CurrentPreview)
        };

        var footer = new Panel(Align.Center(footerRenderable)) { Border = BoxBorder.None };
        return new Layout("Root")
            .SplitRows(
                new Layout("Header").Update(header).Size(3),
                new Layout("Body").Update(body),
                new Layout("Footer").Update(footer).Size(3)
            );
    }

    private static IRenderable CreateSortMenuBody(
        List<(string Text, SortBy By, SortDirection Dir, bool Group)> options,
        int selectedIndex)
    {
        var table = new Table()
            .Title("[bold]Sort Options[/]")
            .Border(TableBorder.Rounded)
            .BorderStyle("yellow");

        table.AddColumn("Options");

        for (var i = 0; i < options.Count; i++)
        {
            var option = options[i];
            var style = i == selectedIndex ? new Style(background: Color.DodgerBlue1) : Style.Plain;
            table.AddRow(new Markup(option.Text.EscapeMarkup(), style));
        }

        return new Align(table, HorizontalAlignment.Center, VerticalAlignment.Middle);
    }

    private static Panel CreateHeader(string currentPath)
    {
        var displayPath = currentPath.Length > 80 ? "..." + currentPath[^77..] : currentPath;
        var headerContent = new Markup($"[bold cyan3]\uE5FF {displayPath.EscapeMarkup()}[/]");
        return new Panel(headerContent) { Border = BoxBorder.Rounded, BorderStyle = new Style(Color.Cyan1) };
    }

    private Layout CreateFileBrowserBody(FileManagerState state, IRenderable previewContent)
    {
        var fileTable = CreateFileTable(state);
        return new Layout("Body").SplitColumns(
            new Layout("FileList").Update(fileTable).Ratio(3),
            new Layout("Preview").Update(previewContent).Ratio(3)
        );
    }

    private Table CreateFileTable(FileManagerState state)
    {
        var table = new Table().Expand().Border(TableBorder.None);
        table.AddColumn("Name");
        table.AddColumn(new TableColumn("Size").RightAligned());
        table.AddColumn(new TableColumn("Modified").RightAligned());
        table.AddColumn(new TableColumn("").Width(1));

        var pageSize = Console.WindowHeight - 12;
        pageSize = Math.Max(5, pageSize);
        var visibleItems = state.CurrentItems.Skip(state.ViewOffset).Take(pageSize).ToList();

        for (var i = 0; i < visibleItems.Count; i++)
        {
            var item = visibleItems[i];
            var originalIndex = i + state.ViewOffset;
            var isHighlighted = originalIndex == state.SelectedIndex;
            var isVisuallySelected = state.VisuallySelectedItems.Contains(item.Path);

            var style = isHighlighted ? new Style(background: Color.DodgerBlue1)
                : isVisuallySelected ? new Style(background: Color.Grey30)
                : Style.Plain;

            var name = CreateNameMarkup(item, isVisuallySelected);
            var scrollChar = GetScrollbarChar(i, state.CurrentItems.Count, pageSize, state.ViewOffset,
                visibleItems.Count);

            table.AddRow(
                new Markup(name, style),
                new Markup(item.FormattedSize, style),
                new Markup(item.FormattedDate, style),
                new Markup(scrollChar, style)
            );
        }

        return table;
    }

    private static string GetScrollbarChar(int currentIndex, int totalItems, int pageSize, int viewOffset,
        int visibleCount)
    {
        if (totalItems <= pageSize) return " ";

        if (currentIndex == 0 && viewOffset > 0) return "⬆";
        if (currentIndex == visibleCount - 1 && viewOffset + pageSize < totalItems) return "⬇";

        var thumbStart = (int)((double)viewOffset / totalItems * visibleCount);
        var thumbEnd = (int)((double)(viewOffset + pageSize) / totalItems * visibleCount);
        if (currentIndex >= thumbStart && currentIndex <= thumbEnd) return "█";

        return "║";
    }

    private string CreateNameMarkup(FileSystemItem item, bool isVisuallySelected)
    {
        var icon = iconProvider.GetIcon(item);
        var name = item.Name.EscapeMarkup();
        var nameStyle = item.IsDirectory ? "bold" : "";
        var selectionMarker = isVisuallySelected ? "[yellow]*[/]" : " ";
        return $"{selectionMarker} {icon}  [{nameStyle}]{name}[/]";
    }

    public static void ShowError(string message)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold red]Error:[/] [red]{message.EscapeMarkup()}[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    private static IRenderable CreateHelpScreen(FileManagerState state)
    {
        var pageSize = Console.WindowHeight - 12;
        pageSize = Math.Max(5, pageSize);

        var totalItems = HelpProvider.Keybindings.Count;
        var maxOffset = Math.Max(0, totalItems - pageSize);

        var title = "[bold yellow]Keybindings[/]";
        if (state.HelpVerticalOffset > 0) title += " [grey]⬆[/]";
        if (state.HelpVerticalOffset < maxOffset) title += " [grey]⬇[/]";

        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded)
            .BorderStyle("yellow")
            .Expand();

        table.AddColumn("[u]Key(s)[/]");
        table.AddColumn("[u]Action[/]");
        table.AddColumn("[u]Mode[/]");

        var visibleItems = HelpProvider.Keybindings
            .Skip(state.HelpVerticalOffset)
            .Take(pageSize);

        foreach (var (keys, action, mode) in visibleItems)
        {
            if (keys.StartsWith("--") && keys.EndsWith("--"))
            {
                table.AddRow(new Markup($"[bold]{keys.Replace("--", "").Trim()}[/]", "yellow")).Centered();
            }
            else
            {
                table.AddRow(keys, action, mode);
            }
        }

        return new Align(table, HorizontalAlignment.Center, VerticalAlignment.Middle);
    }
}