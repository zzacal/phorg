using Phorg.Core;

var srcPath = "/Volumes/Transcend/DCIM/";
var destBasePath = "/Users/zzacal/Pictures";

Console.WriteLine($"source: {srcPath}");

var files = Recon.GetFilesRecursively(srcPath);
var jobs = JobHelpers.CreateJobs(files, destBasePath);

foreach (var job in jobs)
{
    FileStore.Copy(job.Value.Sources, job.Value.Folder, true);
}

Console.WriteLine("Complete");
