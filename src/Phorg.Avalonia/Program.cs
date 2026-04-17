using Avalonia;
using Phorg.Avalonia;

AppBuilder
    .Configure<App>()
    .UsePlatformDetect()
    .StartWithClassicDesktopLifetime(args);
