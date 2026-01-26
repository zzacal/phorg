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
}
