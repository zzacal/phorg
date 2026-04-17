using Avalonia.Controls;
using Phorg.Avalonia.ViewModels;

namespace Phorg.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            if (DataContext is MainViewModel vm)
                vm.Owner = this;
        };
    }
}
