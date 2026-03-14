using Phorg.Core;

namespace Phorg;

public class Processor(JobHelpers Jobber, FileStore FileStore)
{
    public Dictionary<string, Movables> Prepare(string source, string destination)
    {        
        var files = Recon.GetFilesRecursively(source);
        var jobs = Jobber.CreateJobs(files, destination);
        return jobs;
    }
    public int Start(KeyValuePair<string, Movables> job, Action<string> fileCopySucceededHandler, Action<string> fileCopyFailedHandler, bool dryrun = false)
    {
        FileStore.Copy(job.Value.Sources, job.Value.Folder, fileCopySucceededHandler, fileCopyFailedHandler, dryrun);
        return job.Value.Sources.Count();
    }
}
