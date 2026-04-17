using Phorg.Avalonia.ViewModels;

namespace Phorg.Avalonia.Tests;

public class DateGroupViewModelTests
{
    [Fact]
    public void Constructor_SetsDateKeyAndFileCount()
    {
        var files = new List<FileInfo> { new("a.jpg"), new("b.jpg") };

        var vm = new DateGroupViewModel("20240715", files);

        Assert.Equal("20240715", vm.DateKey);
        Assert.Equal(2, vm.FileCount);
    }

    [Fact]
    public void Constructor_StoresSources()
    {
        var files = new List<FileInfo> { new("a.jpg") };

        var vm = new DateGroupViewModel("20240715", files);

        Assert.Same(files, vm.Sources);
    }

    [Fact]
    public void Suffix_DefaultsToEmpty()
    {
        var vm = new DateGroupViewModel("20240715", []);

        Assert.Equal(string.Empty, vm.Suffix);
    }

    [Fact]
    public void Suffix_RaisesPropertyChanged()
    {
        var vm = new DateGroupViewModel("20240715", []);
        var raised = false;
        vm.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(vm.Suffix)) raised = true; };

        vm.Suffix = "Beach Trip";

        Assert.True(raised);
        Assert.Equal("Beach Trip", vm.Suffix);
    }
}
