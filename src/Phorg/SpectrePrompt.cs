using Phorg.Core;
using Spectre.Console;

namespace Phorg;

public class SpecterPrompt : IPrompt
{
    public T Ask<T>(string question, T? def = default)
    {
        var result = def switch
        {
            T => AnsiConsole.Ask(question, def),
            _ => AnsiConsole.Ask<T>(question)
        };
        
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

    public void Warn(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]{message}[/]");
    }
}
