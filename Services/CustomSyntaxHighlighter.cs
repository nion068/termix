using Spectre.Console;
using System.Text;
using System.Text.RegularExpressions;

public record LanguageTheme
{
    public Dictionary<string, Style> Keywords { get; init; } = new();
    public Style StringStyle { get; init; } = new(Color.Yellow);
    public Style CommentStyle { get; init; } = new(Color.Green);
    public Style NumberStyle { get; init; } = new(Color.Magenta1);
    public Style TypeStyle { get; init; } = new(Color.Cyan1);
}

public class CustomSyntaxHighlighter
{
    private readonly Dictionary<string, LanguageTheme> _themes = new();

    public CustomSyntaxHighlighter()
    {
        _themes["csharp"] = new LanguageTheme
        {
            Keywords = new()
            {
                ["public"] = new(Color.Blue),
                ["private"] = new(Color.Blue),
                ["protected"] = new(Color.Blue),
                ["class"] = new(Color.Blue),
                ["void"] = new(Color.Blue),
                ["string"] = new(Color.Blue),
                ["int"] = new(Color.Blue),
                ["bool"] = new(Color.Blue),
                ["var"] = new(Color.Blue),
                ["if"] = new(Color.Purple),
                ["else"] = new(Color.Purple),
                ["for"] = new(Color.Purple),
                ["foreach"] = new(Color.Purple),
                ["while"] = new(Color.Purple),
                ["return"] = new(Color.Purple),
                ["new"] = new(Color.Purple),
                ["using"] = new(Color.Grey),
                ["get"] = new(Color.DarkCyan),
                ["set"] = new(Color.DarkCyan)
            },
            TypeStyle = new(Color.Teal)
        };

        _themes["python"] = new LanguageTheme
        {
            Keywords = new()
            {
                ["def"] = new(Color.Blue),
                ["class"] = new(Color.Blue),
                ["if"] = new(Color.Purple),
                ["else"] = new(Color.Purple),
                ["elif"] = new(Color.Purple),
                ["for"] = new(Color.Purple),
                ["while"] = new(Color.Purple),
                ["return"] = new(Color.Purple),
                ["import"] = new(Color.Grey),
                ["from"] = new(Color.Grey),
                ["and"] = new(Color.Orange1),
                ["or"] = new(Color.Orange1)
            },
            CommentStyle = new Style(Color.Green, decoration: Decoration.Italic),
            TypeStyle = new(Color.Teal)
        };

        _themes["javascript"] = new LanguageTheme
        {
            Keywords = new()
            {
                ["function"] = new(Color.Blue),
                ["class"] = new(Color.Blue),
                ["const"] = new(Color.Blue),
                ["let"] = new(Color.Blue),
                ["var"] = new(Color.Blue),
                ["if"] = new(Color.Purple),
                ["else"] = new(Color.Purple),
                ["for"] = new(Color.Purple),
                ["while"] = new(Color.Purple),
                ["return"] = new(Color.Purple),
                ["import"] = new(Color.Grey),
                ["from"] = new(Color.Grey),
                ["export"] = new(Color.Grey)
            },
            TypeStyle = new(Color.Teal)
        };
    }

    public Markup Highlight(string code, string language)
    {
        if (!_themes.TryGetValue(language, out var theme))
        {
            return new Markup(code.EscapeMarkup());
        }

        var sb = new StringBuilder();
        var lines = code.Split('\n');

        foreach (var line in lines)
        {
            var processedLine = line.EscapeMarkup();

            processedLine = Regex.Replace(processedLine, "(\\\".*?\\\")", m => $"[{theme.StringStyle.ToMarkup()}]{m.Value}[/]");

            processedLine = Regex.Replace(processedLine, "(//.*)", m => $"[{theme.CommentStyle.ToMarkup()}]{m.Value}[/]");

            processedLine = Regex.Replace(processedLine, @"\b\d+\b", m => $"[{theme.NumberStyle.ToMarkup()}]{m.Value}[/]");

            var keywordRegex = new Regex(@"\b(" + string.Join("|", theme.Keywords.Keys) + @")\b");
            processedLine = keywordRegex.Replace(processedLine, m =>
            {
                var keyword = m.Value;
                var style = theme.Keywords[keyword];
                return $"[{style.ToMarkup()}]{keyword}[/]";
            });

            processedLine = Regex.Replace(processedLine, @"\b([A-Z][a-zA-Z0-9_]*)\b", m => $"[{theme.TypeStyle.ToMarkup()}]{m.Value}[/]");

            sb.AppendLine(processedLine);
        }

        return new Markup(sb.ToString());
    }
}