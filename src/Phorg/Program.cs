using Phorg;
using Phorg.Core;

var prompt = new SpecterPrompt();
var jobber = new JobHelpers(prompt);
var processor = new Processor(jobber);

var source = "/Volumes/Transcend/DCIM/";
var destination = "/Users/zzacal/Pictures";

prompt.Say($"source: {source}");
var copied = processor.Start(source, destination, false);

prompt.Success($"✓ Copied {copied} successfully");
