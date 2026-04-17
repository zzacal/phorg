using Moq;
using Phorg.Core;

namespace Phorg.Tests;

public class ProcessorTests : IDisposable
{
    private readonly string _srcDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly string _destDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public ProcessorTests() => Directory.CreateDirectory(_srcDir);

    public void Dispose()
    {
        if (Directory.Exists(_srcDir)) Directory.Delete(_srcDir, recursive: true);
        if (Directory.Exists(_destDir)) Directory.Delete(_destDir, recursive: true);
    }

    private FileInfo CreateFile(string name, DateTime creationTime)
    {
        var path = Path.Combine(_srcDir, name);
        File.WriteAllText(path, "");
        File.SetCreationTime(path, creationTime);
        return new FileInfo(path);
    }

    [Fact]
    public void Prepare_ReturnsJobsGroupedByDate()
    {
        CreateFile("a.jpg", new DateTime(2024, 7, 15));
        CreateFile("b.jpg", new DateTime(2024, 8, 1));
        var prompt = new Mock<IPrompt>();
        prompt.Setup(p => p.Ask(It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);
        var processor = new Processor(new JobHelpers(prompt.Object), new FileStore());

        var jobs = processor.Prepare(_srcDir, _destDir);

        Assert.Equal(2, jobs.Count);
    }

    [Fact]
    public void Start_ReturnsSourceCount()
    {
        CreateFile("a.jpg", new DateTime(2024, 7, 15));
        CreateFile("b.jpg", new DateTime(2024, 7, 15));
        var prompt = new Mock<IPrompt>();
        prompt.Setup(p => p.Ask(It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);
        var processor = new Processor(new JobHelpers(prompt.Object), new FileStore());
        var jobs = processor.Prepare(_srcDir, _destDir);

        var count = processor.Start(jobs.First(), _ => { }, _ => { }, dryrun: true);

        Assert.Equal(2, count);
    }
}
