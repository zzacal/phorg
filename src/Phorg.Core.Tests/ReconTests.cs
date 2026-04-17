using Phorg.Core;

namespace Phorg.Core.Tests;

public class ReconTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public ReconTests() => Directory.CreateDirectory(_tempDir);

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    [Fact]
    public void GetFilesRecursively_ReturnsFilesInDirectory()
    {
        File.WriteAllText(Path.Combine(_tempDir, "a.jpg"), "");
        File.WriteAllText(Path.Combine(_tempDir, "b.jpg"), "");

        var result = Recon.GetFilesRecursively(_tempDir);

        Assert.Equal(2, result.Length);
    }

    [Fact]
    public void GetFilesRecursively_ReturnsFilesFromSubdirectories()
    {
        var sub = Directory.CreateDirectory(Path.Combine(_tempDir, "sub"));
        File.WriteAllText(Path.Combine(_tempDir, "root.jpg"), "");
        File.WriteAllText(Path.Combine(sub.FullName, "nested.jpg"), "");

        var result = Recon.GetFilesRecursively(_tempDir);

        Assert.Equal(2, result.Length);
    }

    [Fact]
    public void GetFilesRecursively_EmptyDirectory_ReturnsEmpty()
    {
        var result = Recon.GetFilesRecursively(_tempDir);

        Assert.Empty(result);
    }

    [Fact]
    public void GroupByDate_GroupsFilesByCreationDate()
    {
        var file1 = Path.Combine(_tempDir, "a.jpg");
        var file2 = Path.Combine(_tempDir, "b.jpg");
        var file3 = Path.Combine(_tempDir, "c.jpg");
        File.WriteAllText(file1, "");
        File.WriteAllText(file2, "");
        File.WriteAllText(file3, "");
        File.SetCreationTime(file1, new DateTime(2024, 7, 15));
        File.SetCreationTime(file2, new DateTime(2024, 7, 15));
        File.SetCreationTime(file3, new DateTime(2024, 8, 1));

        var files = new[] { new FileInfo(file1), new FileInfo(file2), new FileInfo(file3) };
        var result = Recon.GroupByDate(files);

        Assert.Equal(2, result.Count);
        Assert.Equal(2, result["20240715"].Count);
        Assert.Single(result["20240801"]);
    }

    [Fact]
    public void GroupByDate_OrdersByDateKey()
    {
        var file1 = Path.Combine(_tempDir, "a.jpg");
        var file2 = Path.Combine(_tempDir, "b.jpg");
        File.WriteAllText(file1, "");
        File.WriteAllText(file2, "");
        File.SetCreationTime(file1, new DateTime(2024, 8, 1));
        File.SetCreationTime(file2, new DateTime(2024, 1, 1));

        var files = new[] { new FileInfo(file1), new FileInfo(file2) };
        var result = Recon.GroupByDate(files);

        Assert.Equal("20240101", result.Keys.First());
        Assert.Equal("20240801", result.Keys.Last());
    }

    [Fact]
    public void GroupByDate_EmptyArray_ReturnsEmptyDictionary()
    {
        var result = Recon.GroupByDate([]);

        Assert.Empty(result);
    }
}
