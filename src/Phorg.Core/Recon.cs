namespace Phorg.Core;

public static class Recon
{
    public static FileInfo[] GetFilesRecursively(string path)
    {
        var files = Directory.GetFiles(path);
        var subDirs = Directory.GetDirectories(path);
        var subFiles = subDirs.SelectMany(GetFilesRecursively);

        var infos = files.Select(x => new FileInfo(x));
        return subFiles.Concat(infos).ToArray();
    }

    public static Dictionary<string, List<FileInfo>> GroupByDate(FileInfo[] files)
        => files
            .GroupBy(f => f.CreationTime.ToString("yyyyMMdd"))
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());
}
