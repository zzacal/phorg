using Phorg.Core;

namespace Phorg;

public record Movables(string Folder, List<FileInfo> Sources);

public class JobHelpers(IPrompt Prompt)
{
    public Dictionary<string, Movables> CreateJobs(FileInfo[] files, string basePath)
    {
        var groups = Recon.GroupByDate(files);
        var targets = new Dictionary<string, Movables>();
        string suffix = string.Empty;

        foreach (var (key, sources) in groups)
        {
            Prompt.Say($"Date: {key}");
            suffix = Prompt.Ask($"Suffix", suffix);
            var folder = $"{basePath}/{$"{key} {suffix}".Trim()}";
            Prompt.Say($"Folder: {folder}");
            targets[key] = new Movables(folder, sources);
        }

        return targets;
    }
}
