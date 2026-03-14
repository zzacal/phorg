using Spectre.Console;

namespace Phorg;

public class SpecterPrompt : IPrompt
{
    private const string SelectCurrent = "→ [[Select this directory]]";
    private const string GoUp = "..";

    public T Ask<T>(string question, T? def = default)
    {
        var result = def switch
        {
            T => AnsiConsole.Ask(question, def),
            _ => AnsiConsole.Ask<T>(question)
        };
        
        return result;
    }

    public string BrowsePath(string question, string startPath)
    {
        var current = Path.GetFullPath(startPath);

        while (true)
        {
            var choices = new List<string> { SelectCurrent };

            if (Directory.GetParent(current) is not null)
                choices.Add(GoUp);

            try
            {
                foreach (var dir in Directory.GetDirectories(current).OrderBy(d => d))
                    choices.Add(Markup.Escape(Path.GetFileName(dir)));
            }
            catch (UnauthorizedAccessException) { }

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[blue]{question}:[/] [yellow]{Markup.Escape(current)}[/]")
                    .PageSize(15)
                    .AddChoices(choices));

            if (selection == SelectCurrent)
                return current;

            current = selection == GoUp
                ? Directory.GetParent(current)!.FullName
                : Path.Combine(current, selection);
        }
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
