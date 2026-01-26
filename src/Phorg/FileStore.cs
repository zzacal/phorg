using Spectre.Console;

namespace Phorg.Core;

public static class FileStore
{
    public static void Copy(IEnumerable<FileInfo> files, string destDir, bool dryrun = false)
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
                    task.Description = $"{destDir}, {completed}/{files.Count()}";
                    Copy(file, destDir, dryrun);
                    task.Increment(1);
                    completed++;
                }
            });
    }

    public static void Copy(FileInfo file, string destDir, bool dryrun = false)
    {
        var dest = $"{destDir}/{file.Name}";
        if (!dryrun)
        {
            File.Copy(file.FullName, dest, false);
        }
    }
}
