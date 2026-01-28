using Phorg;
using Phorg.Core;
using Spectre.Console;

var prompt = new SpecterPrompt();
var jobber = new JobHelpers(prompt);
var fileStore = new FileStore(prompt);
var processor = new Processor(jobber, fileStore);

var source = prompt.Ask("Source", "/Volumes/Transcend/DCIM/");
var destination = prompt.Ask("Destination","/Users/zzacal/Pictures");

prompt.Say($"{source} -->> {destination}");

prompt.Warn("Starting");
var copied = 0;
var jobs = processor.Prepare(source, destination);
AnsiConsole
    .Progress()
    .Start(ctx =>
    {
        var tasks = jobs.ToDictionary
        (
            job => job.Key,
            job => ctx.AddTask($"0/{job.Value.Sources.Count()} {job.Key}", maxValue: job.Value.Sources.Count())
        );

        foreach(var job in jobs)
        {
            var jobCount = job.Value.Sources.Count();
            var completed = 0;
            var jobCopied = processor.Start
            (
                job: job, 
                completedEvent: (file) =>
                {
                    tasks[job.Key].Description = $"{completed++}/{jobCount} {job.Value.Folder}/{file}";
                    tasks[job.Key].Increment(1);
                },
                dryrun: true
            );

            copied += jobCopied;
        }
    });

prompt.Success($"✓ Copied {copied} successfully");
