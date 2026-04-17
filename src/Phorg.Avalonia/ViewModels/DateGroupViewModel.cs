using CommunityToolkit.Mvvm.ComponentModel;

namespace Phorg.Avalonia.ViewModels;

public partial class DateGroupViewModel : ObservableObject
{
    public string DateKey { get; }
    public int FileCount { get; }
    public List<FileInfo> Sources { get; }

    [ObservableProperty] private string _suffix = string.Empty;

    public DateGroupViewModel(string dateKey, List<FileInfo> sources)
    {
        DateKey = dateKey;
        FileCount = sources.Count;
        Sources = sources;
    }
}
