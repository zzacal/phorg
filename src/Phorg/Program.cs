using Phorg;
using Phorg.Core;

var prompt = new SpecterPrompt();
var jobber = new JobHelpers(prompt);
var fileStore = new FileStore(prompt);
var processor = new Processor(jobber, fileStore);

var source = prompt.Ask("Source", "/Volumes/Transcend/DCIM/");
var destination = prompt.Ask("Destination","/Users/zzacal/Pictures");

prompt.Say($"{source} -->> {destination}");

prompt.Warn("Starting");

var copied = processor.Start(source, destination, false);

prompt.Success($"✓ Copied {copied} successfully");
