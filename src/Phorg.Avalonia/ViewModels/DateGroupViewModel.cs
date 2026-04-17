using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Phorg.Avalonia.ViewModels;

public partial class DateGroupViewModel : ObservableObject
{
    public string DateKey { get; }
    public int FileCount { get; }
    public List<FileInfo> Sources { get; }

    [ObservableProperty] private string _suffix = string.Empty;
    [ObservableProperty] private bool _hasPreviews;

    public ObservableCollection<Bitmap> Previews { get; } = new();

    private static readonly HashSet<string> _imageExts = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

    public DateGroupViewModel(string dateKey, List<FileInfo> sources)
    {
        DateKey = dateKey;
        FileCount = sources.Count;
        Sources = sources;
    }

    public async Task LoadPreviewsAsync()
    {
        var imageFiles = Sources
            .Where(f => _imageExts.Contains(f.Extension))
            .Take(10)
            .ToList();

        foreach (var file in imageFiles)
        {
            try
            {
                var bitmap = await Task.Run(() =>
                {
                    using var stream = File.OpenRead(file.FullName);
                    return Bitmap.DecodeToWidth(stream, 120);
                });
                Previews.Add(bitmap);
                HasPreviews = true;
            }
            catch { }
        }
    }
}
