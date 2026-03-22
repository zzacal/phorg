namespace Phorg.Core;

public class FileStore()
{
    public void Copy(IEnumerable<FileInfo> files, string destDir, Action<string> fileCopySucceededHandler, Action<string> fileCopyFailedHandler, bool dryrun = false)
    {
        if(!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 2 }, file =>
        {
            try
            {
                Copy(file, destDir, dryrun);
                fileCopySucceededHandler(file.Name);
            }
            catch
            {
                fileCopyFailedHandler(file.Name);
            }
        });
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
