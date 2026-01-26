using Phorg.Core;
using Spectre.Console;

namespace Phorg;

public class SpecterPrompt : IPrompt
{
    public T Ask<T>(string question, T? def = default)
    {
        var result = AnsiConsole.Ask<T>(question, def);
        Console.WriteLine(result);
        return result;
    }

    public void Say(string message)
    {
        AnsiConsole.MarkupLine($"[blue]{message}[/]");
    }
    
    public void Success(string message)
    {
        AnsiConsole.MarkupLine($"[green]{message}[/]");
    }
}
