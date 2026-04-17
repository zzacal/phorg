using Phorg.Avalonia.ViewModels;

namespace Phorg.Avalonia.Tests;

public class MainViewModelTests
{
    private static MainViewModel Make() => new(dispatch: action => action());

    [Fact]
    public void InitialState_HasEmptyPathsAndNoGroups()
    {
        var vm = Make();

        Assert.Equal(string.Empty, vm.SourcePath);
        Assert.Equal(string.Empty, vm.DestPath);
        Assert.Empty(vm.DateGroups);
        Assert.False(vm.IsScanning);
        Assert.False(vm.IsCopying);
        Assert.Equal(0, vm.CopiedCount);
        Assert.Equal(string.Empty, vm.Log);
    }

    [Fact]
    public async Task Scan_EmptySourcePath_DoesNotPopulateGroups()
    {
        var vm = Make();
        vm.SourcePath = string.Empty;

        await vm.ScanCommand.ExecuteAsync(null);

        Assert.Empty(vm.DateGroups);
    }

    [Fact]
    public async Task Scan_ValidPath_PopulatesDateGroups()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            var file1 = Path.Combine(tempDir, "a.jpg");
            var file2 = Path.Combine(tempDir, "b.jpg");
            File.WriteAllText(file1, "");
            File.WriteAllText(file2, "");
            File.SetCreationTime(file1, new DateTime(2024, 7, 15));
            File.SetCreationTime(file2, new DateTime(2024, 8, 1));

            var vm = Make();
            vm.SourcePath = tempDir;

            await vm.ScanCommand.ExecuteAsync(null);

            Assert.Equal(2, vm.DateGroups.Count);
            Assert.False(vm.IsScanning);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task Scan_LogsFileCount()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            File.WriteAllText(Path.Combine(tempDir, "a.jpg"), "");

            var vm = Make();
            vm.SourcePath = tempDir;
            await vm.ScanCommand.ExecuteAsync(null);

            Assert.Contains("1 files", vm.Log);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task StartCopy_WithNoGroups_DoesNotCopy()
    {
        var vm = Make();
        vm.DestPath = "/some/dest";

        await vm.StartCopyCommand.ExecuteAsync(null);

        Assert.Equal(0, vm.CopiedCount);
    }

    [Fact]
    public async Task StartCopy_WithNoDestPath_DoesNotCopy()
    {
        var vm = Make();
        vm.DateGroups.Add(new DateGroupViewModel("20240715", []));

        await vm.StartCopyCommand.ExecuteAsync(null);

        Assert.Equal(0, vm.CopiedCount);
    }

    [Fact]
    public async Task StartCopy_CopiesFilesAndUpdatesCopiedCount()
    {
        var srcDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var destDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(srcDir);
        try
        {
            var file = Path.Combine(srcDir, "photo.jpg");
            File.WriteAllText(file, "data");

            var vm = Make();
            vm.DestPath = destDir;
            vm.DateGroups.Add(new DateGroupViewModel("20240715", [new FileInfo(file)]) { Suffix = "Vacation" });

            await vm.StartCopyCommand.ExecuteAsync(null);

            Assert.Equal(1, vm.CopiedCount);
            Assert.True(File.Exists(Path.Combine(destDir, "20240715 Vacation", "photo.jpg")));
        }
        finally
        {
            if (Directory.Exists(srcDir)) Directory.Delete(srcDir, recursive: true);
            if (Directory.Exists(destDir)) Directory.Delete(destDir, recursive: true);
        }
    }
}
