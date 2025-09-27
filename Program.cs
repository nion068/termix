using Spectre.Console;

namespace termix;

public static class Program
{
    public static Task Main(string[] args)
    {
        var useIcons = !args.Contains("--no-icons");
        try
        {
            AnsiConsole.Clear();
            var fileManager = new FileManager(useIcons);
            fileManager.Run();

            AnsiConsole.Clear();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"{ex.GetType().Name}: [red]{ex.Message}[/]");

            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                AnsiConsole.WriteLine(ex.StackTrace);
            }
        }

        return Task.CompletedTask;
    }
}