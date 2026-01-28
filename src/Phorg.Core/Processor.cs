namespace Phorg.Core;

public class Processor(JobHelpers Jobber, FileStore FileStore)
{
    public int Start(string source, string destination, bool dryrun = false)
    {
        var files = Recon.GetFilesRecursively(source);
        var jobs = Jobber.CreateJobs(files, destination);

        foreach (var job in jobs)
        {
            FileStore.Copy(job.Value.Sources, job.Value.Folder, dryrun);
        }

        return files.Length;
    }
}
