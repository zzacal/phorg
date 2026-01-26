namespace Phorg.Core;

public static class FileStore
{
    public static void Copy(IEnumerable<FileInfo> files, string destDir, bool dryrun = false)
    {
        foreach(var file in files)
        {
            Copy(file, destDir, dryrun);
        }
    }
    public static void Copy(FileInfo file, string destDir,  bool dryrun = false)
    {
        var dest = $"{destDir}/{file.Name}";
        try
        {   
            if(dryrun)
            {
                Console.WriteLine(dest);
            }
            else
            {
                File.Copy(file.FullName, dest);
            }
        }
        catch(DirectoryNotFoundException)
        {
            Directory.CreateDirectory(destDir);
        }
    }
}
