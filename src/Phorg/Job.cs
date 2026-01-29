namespace Phorg;

public record Movables(string Folder, List<FileInfo> Sources);

public class JobHelpers(IPrompt Prompt)
{
    public Dictionary<string, Movables> CreateJobs(FileInfo[] files, string basePath)
    {
        var targets  = new Dictionary<string, Movables>();
        string suffix = string.Empty;
        
        foreach (var file in files)
        {
            var key = file.CreationTime.ToString("yyyyMMdd");
            if(!targets.ContainsKey(key))
            {
                Prompt.Say($"Date: {key}");                
                suffix = Prompt.Ask($"Suffix", suffix);

                var folder = $"{basePath}/{$"{key} {suffix}".Trim()}";
                targets[key] = new Movables(folder, new ());
                Prompt.Say($"Folder: {folder}");
            }

            targets[key].Sources.Add(file);
        }

        return targets;
    }
}
