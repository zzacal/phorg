namespace Phorg.Core;

public class FileStore()
{
    public void Copy(IEnumerable<FileInfo> files, string destDir, Action<string> completedEvent, bool dryrun = false)
    {
        if(!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        foreach (var file in files)
        {
            Copy(file, destDir, dryrun);
            completedEvent(file.Name);
        }        
    }

    public void Copy(FileInfo file, string destDir, bool dryrun = false)
    {
        var dest = $"{destDir}/{file.Name}";
        if (!dryrun)
        {
            File.Copy(file.FullName, dest, false);
        }
    }
}
