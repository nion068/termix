using Spectre.Console;
using Spectre.Console.Rendering;

namespace termix.UI;

public class DoubleBufferedRenderer
{
    private readonly StringWriter _writer;
    private readonly IAnsiConsole _console;

    public DoubleBufferedRenderer()
    {
        _writer = new StringWriter();

        _console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            ColorSystem = ColorSystemSupport.TrueColor,
            Out = new AnsiConsoleOutput(_writer),
        });
    }

    public void Render(IRenderable layout)
    {
        _writer.GetStringBuilder().Clear();
        _console.Write(layout);
        var output = _writer.ToString();
        Console.SetCursorPosition(0, 0);
        Console.Write(output);
    }
}