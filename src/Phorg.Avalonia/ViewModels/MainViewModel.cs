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
    partial void OnIsScanningChanged(bool value) => ScanCommand.NotifyCanExecuteChanged();
    partial void OnDestPathChanged(string value) => StartCopyCommand.NotifyCanExecuteChanged();
    partial void OnIsCopyingChanged(bool value) => StartCopyCommand.NotifyCanExecuteChanged();
    [ObservableProperty] private int _copiedCount;
    [ObservableProperty] private string _log = string.Empty;

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
        Log = string.Empty;

        try
        {
            var files = await Task.Run(() => Recon.GetFilesRecursively(SourcePath));
            var groups = Recon.GroupByDate(files);

            foreach (var (key, sources) in groups)
                DateGroups.Add(new DateGroupViewModel(key, sources));

            AppendLog($"Found {files.Length} files across {groups.Count} date groups.");
        }
        catch (Exception ex)
        {
            AppendLog($"Scan failed: {ex.Message}");
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
        Log = string.Empty;

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
                        name => _dispatch(() =>
                        {
                            CopiedCount++;
                            AppendLog($"Copied: {folderName}/{name}");
                        }),
                        name => _dispatch(() =>
                            AppendLog($"Failed: {folderName}/{name}"))
                    );
                }
            });

            AppendLog($"Done. {CopiedCount} files copied.");
        }
        catch (Exception ex)
        {
            AppendLog($"Copy failed: {ex.Message}");
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

    private void AppendLog(string message) =>
        Log = string.IsNullOrEmpty(Log) ? message : $"{Log}\n{message}";
}
