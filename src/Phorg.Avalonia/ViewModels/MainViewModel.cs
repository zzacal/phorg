using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Phorg.Core;

namespace Phorg.Avalonia.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Action<Action> _dispatch;

    public MainViewModel(Action<Action>? dispatch = null)
    {
        _dispatch = dispatch ?? (action => Dispatcher.UIThread.Post(action));
        DateGroups.CollectionChanged += (_, _) => StartCopyCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty] private string _sourcePath = string.Empty;
    [ObservableProperty] private string _destPath = string.Empty;
    [ObservableProperty] private bool _isScanning;
    [ObservableProperty] private bool _isCopying;

    partial void OnSourcePathChanged(string value) => ScanCommand.NotifyCanExecuteChanged();
    partial void OnIsScanningChanged(bool value) { ScanCommand.NotifyCanExecuteChanged(); OnPropertyChanged(nameof(HasProgress)); }
    partial void OnDestPathChanged(string value) => StartCopyCommand.NotifyCanExecuteChanged();
    partial void OnIsCopyingChanged(bool value) { StartCopyCommand.NotifyCanExecuteChanged(); OnPropertyChanged(nameof(HasProgress)); }
    [ObservableProperty] private int _copiedCount;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private string _lastCopiedFile = string.Empty;

    partial void OnTotalCountChanged(int value) => OnPropertyChanged(nameof(HasProgress));

    public bool HasProgress => IsScanning || IsCopying || TotalCount > 0;

    public ObservableCollection<DateGroupViewModel> DateGroups { get; } = new();

    public Window? Owner { get; set; }

    [RelayCommand]
    private async Task BrowseSource()
    {
        var path = await PickFolder("Select Source Folder");
        if (path is not null)
        {
            SourcePath = path;
            DateGroups.Clear();
        }
    }

    [RelayCommand]
    private async Task BrowseDest()
    {
        var path = await PickFolder("Select Destination Folder");
        if (path is not null)
            DestPath = path;
    }

    [RelayCommand(CanExecute = nameof(CanScan))]
    private async Task Scan()
    {
        if (string.IsNullOrWhiteSpace(SourcePath)) return;

        IsScanning = true;
        DateGroups.Clear();
        TotalCount = 0;
        CopiedCount = 0;

        try
        {
            var files = await Task.Run(() => Recon.GetFilesRecursively(SourcePath));
            var groups = Recon.GroupByDate(files);

            foreach (var (key, sources) in groups)
                DateGroups.Add(new DateGroupViewModel(key, sources));

            TotalCount = files.Length;
        }
        catch (Exception ex)
        {
            _ = ex;
        }
        finally
        {
            IsScanning = false;
        }
    }

    private bool CanScan() => !IsScanning && !string.IsNullOrWhiteSpace(SourcePath);

    [RelayCommand(CanExecute = nameof(CanStartCopy))]
    private async Task StartCopy()
    {
        if (string.IsNullOrWhiteSpace(DestPath) || DateGroups.Count == 0) return;

        IsCopying = true;
        CopiedCount = 0;

        var store = new FileStore();

        try
        {
            await Task.Run(() =>
            {
                foreach (var group in DateGroups)
                {
                    var folderName = $"{group.DateKey} {group.Suffix}".Trim();
                    var destDir = Path.Combine(DestPath, folderName);

                    store.Copy(
                        group.Sources,
                        destDir,
                        name => _dispatch(() => { CopiedCount++; LastCopiedFile = name; }),
                        name => _dispatch(() => { CopiedCount++; LastCopiedFile = name; })
                    );
                }
            });

            await Dispatcher.UIThread.InvokeAsync(() => { });
        }
        catch (Exception ex)
        {
            _ = ex;
        }
        finally
        {
            IsCopying = false;
        }
    }

    private bool CanStartCopy() => !IsCopying && !string.IsNullOrWhiteSpace(DestPath) && DateGroups.Count > 0;

    private async Task<string?> PickFolder(string title)
    {
        if (Owner is null) return null;

        var results = await Owner.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });

        return results.Count > 0 ? results[0].TryGetLocalPath() : null;
    }

}
