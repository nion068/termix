using Spectre.Console;

namespace termix;

public static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            AnsiConsole.Clear();

            var title = new FigletText("Termix")
                .Centered()
                .Color(Color.Cyan1);

            var description = new Panel(
                new Markup("[grey]A sleek and modern file manager.\n\n[dim]Tip: For best experience, use a [underline]Nerd Font[/] terminal.[/][/]")
            )
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1),
                BorderStyle = new Style(Color.Aqua),
                Expand = true
            };

            AnsiConsole.Write(title);
            AnsiConsole.Write(description);

            await Task.Delay(1500);


            var fileManager = new FileManager();
            fileManager.Run();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
        }
        finally
        {

            AnsiConsole.Clear();
            AnsiConsole.Write(
                new Panel("[bold green]Thanks for using Termix![/]\n[dim]Have a productive session 🤖[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(new Style(Color.Green))
                    .Padding(1, 1)
                    .Expand()
            );
        }
    }
}