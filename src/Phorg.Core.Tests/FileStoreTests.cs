using Phorg.Core;

namespace Phorg.Core.Tests;

public class FileStoreTests : IDisposable
{
    private readonly string _srcDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly string _destDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly FileStore _store = new();

    public FileStoreTests() => Directory.CreateDirectory(_srcDir);

    public void Dispose()
    {
        if (Directory.Exists(_srcDir)) Directory.Delete(_srcDir, recursive: true);
        if (Directory.Exists(_destDir)) Directory.Delete(_destDir, recursive: true);
    }

    [Fact]
    public void Copy_CopiesFilesToDestination()
    {
        var path = Path.Combine(_srcDir, "photo.jpg");
        File.WriteAllText(path, "data");
        var files = new[] { new FileInfo(path) };

        _store.Copy(files, _destDir, _ => { }, _ => { });

        Assert.True(File.Exists(Path.Combine(_destDir, "photo.jpg")));
    }

    [Fact]
    public void Copy_CreatesDestinationDirectoryIfMissing()
    {
        var path = Path.Combine(_srcDir, "photo.jpg");
        File.WriteAllText(path, "data");
        var files = new[] { new FileInfo(path) };

        _store.Copy(files, _destDir, _ => { }, _ => { });

        Assert.True(Directory.Exists(_destDir));
    }

    [Fact]
    public void Copy_DryRun_DoesNotCopyFiles()
    {
        var path = Path.Combine(_srcDir, "photo.jpg");
        File.WriteAllText(path, "data");
        var files = new[] { new FileInfo(path) };

        _store.Copy(files, _destDir, _ => { }, _ => { }, dryrun: true);

        Assert.False(File.Exists(Path.Combine(_destDir, "photo.jpg")));
    }

    [Fact]
    public void Copy_InvokesSuccessCallbackForEachFile()
    {
        File.WriteAllText(Path.Combine(_srcDir, "a.jpg"), "");
        File.WriteAllText(Path.Combine(_srcDir, "b.jpg"), "");
        var files = Directory.GetFiles(_srcDir).Select(f => new FileInfo(f)).ToArray();
        var succeeded = new List<string>();

        _store.Copy(files, _destDir, name => succeeded.Add(name), _ => { });

        Assert.Equal(2, succeeded.Count);
    }

    [Fact]
    public void Copy_InvokesFailureCallbackWhenFileAlreadyExists()
    {
        var path = Path.Combine(_srcDir, "photo.jpg");
        File.WriteAllText(path, "data");
        Directory.CreateDirectory(_destDir);
        File.WriteAllText(Path.Combine(_destDir, "photo.jpg"), "existing");
        var files = new[] { new FileInfo(path) };
        var failed = new List<string>();

        _store.Copy(files, _destDir, _ => { }, name => failed.Add(name));

        Assert.Single(failed);
        Assert.Equal("photo.jpg", failed[0]);
    }
}
