using Moq;
using Phorg.Core;

namespace Phorg.Tests;

public class JobHelpersTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public JobHelpersTests() => Directory.CreateDirectory(_tempDir);

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private FileInfo CreateFile(string name, DateTime creationTime)
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, "");
        File.SetCreationTime(path, creationTime);
        return new FileInfo(path);
    }

    [Fact]
    public void CreateJobs_GroupsFilesByDate()
    {
        var prompt = new Mock<IPrompt>();
        prompt.Setup(p => p.Ask(It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);
        var helpers = new JobHelpers(prompt.Object);
        var files = new[]
        {
            CreateFile("a.jpg", new DateTime(2024, 7, 15)),
            CreateFile("b.jpg", new DateTime(2024, 7, 15)),
            CreateFile("c.jpg", new DateTime(2024, 8, 1)),
        };

        var jobs = helpers.CreateJobs(files, "/dest");

        Assert.Equal(2, jobs.Count);
        Assert.Equal(2, jobs["20240715"].Sources.Count);
        Assert.Single(jobs["20240801"].Sources);
    }

    [Fact]
    public void CreateJobs_BuildsFolderPathWithSuffix()
    {
        var prompt = new Mock<IPrompt>();
        prompt.Setup(p => p.Ask(It.IsAny<string>(), It.IsAny<string>())).Returns("Beach Trip");
        var helpers = new JobHelpers(prompt.Object);
        var files = new[] { CreateFile("a.jpg", new DateTime(2024, 7, 15)) };

        var jobs = helpers.CreateJobs(files, "/dest");

        Assert.Equal("/dest/20240715 Beach Trip", jobs["20240715"].Folder);
    }

    [Fact]
    public void CreateJobs_BuildsFolderPathWithoutSuffix_WhenSuffixIsEmpty()
    {
        var prompt = new Mock<IPrompt>();
        prompt.Setup(p => p.Ask(It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);
        var helpers = new JobHelpers(prompt.Object);
        var files = new[] { CreateFile("a.jpg", new DateTime(2024, 7, 15)) };

        var jobs = helpers.CreateJobs(files, "/dest");

        Assert.Equal("/dest/20240715", jobs["20240715"].Folder);
    }

    [Fact]
    public void CreateJobs_CarriesSuffixForwardAsDefault()
    {
        var callCount = 0;
        string? lastDefault = null;
        var prompt = new Mock<IPrompt>();
        prompt.Setup(p => p.Ask(It.IsAny<string>(), It.IsAny<string>()))
              .Returns<string, string>((_, def) =>
              {
                  lastDefault = def;
                  callCount++;
                  return callCount == 1 ? "Vacation" : def;
              });
        var helpers = new JobHelpers(prompt.Object);
        var files = new[]
        {
            CreateFile("a.jpg", new DateTime(2024, 7, 15)),
            CreateFile("b.jpg", new DateTime(2024, 8, 1)),
        };

        helpers.CreateJobs(files, "/dest");

        Assert.Equal("Vacation", lastDefault);
    }

    [Fact]
    public void CreateJobs_EmptyFiles_ReturnsEmpty()
    {
        var prompt = new Mock<IPrompt>();
        var helpers = new JobHelpers(prompt.Object);

        var jobs = helpers.CreateJobs([], "/dest");

        Assert.Empty(jobs);
    }
}
