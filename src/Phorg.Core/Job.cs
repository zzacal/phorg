namespace Phorg.Core;

public record Movables(string Folder, List<FileInfo> Sources);

public static class JobHelpers
{
    public static Dictionary<string, Movables> CreateJobs(FileInfo[] files, string basePath)
    {
        var targets  = new Dictionary<string, Movables>();
        string suffix = string.Empty;
        
        foreach (var file in files)
        {
            var key = file.CreationTime.ToString("yyyyMMdd");
            if(!targets.ContainsKey(key))
            {
                Console.WriteLine($"Folder: {key}");
                Console.Write($"Folder Suffix (\"{suffix}\"): ");
                var newSuffix = Console.ReadLine();
                suffix = string.IsNullOrWhiteSpace(newSuffix) ? suffix : newSuffix;

                var folder = $"{basePath}/{$"{key} {suffix}".Trim()}";
                targets[key] = new Movables(folder, new ());
                Console.WriteLine(folder);
            }

            targets[key].Sources.Add(file);
        }

        return targets;
    }
}
