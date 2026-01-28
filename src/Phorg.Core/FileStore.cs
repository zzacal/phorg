using Spectre.Console;

namespace Phorg.Core;

public class FileStore(IPrompt Prompt)
{
    public void Copy(IEnumerable<FileInfo> files, string destDir, bool dryrun = false)
    {
        if(!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
            AnsiConsole.MarkupLine($"[green]✓ Created {destDir} [/]");
        }
        
        AnsiConsole
            .Progress()
            .Start(ctx =>
            {
                var completed = 0;
                var task = ctx.AddTask($"{destDir}, {completed}/{files.Count()}", maxValue: files.Count());

                foreach (var file in files)
                {
                    Copy(file, destDir, dryrun);
                    completed++;
                    task.Description = $"{destDir}/{file.Name}, {completed}/{files.Count()}";
                    task.Increment(1);
                }
            });
    }

    public void Copy(FileInfo file, string destDir, bool dryrun = false)
    {
        var dest = $"{destDir}/{file.Name}";
        if (!dryrun)
        {
            try
            {                
                File.Copy(file.FullName, dest, false);
            }
            catch (Exception ex)
            {
                Prompt.Warn(ex.Message);
            }
        }
    }
}
