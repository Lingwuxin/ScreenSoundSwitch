using Microsoft.Win32;
using System.Windows.Forms;
using System;

public class DisplayWatcher : IDisposable
{
    public DisplayWatcher()
    {
        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        // 执行你自己的逻辑，比如重新加载布局、刷新窗口等
        Console.WriteLine("Display settings changed. Screens may have been added, removed, or rearranged.");

        // 例如，重新枚举屏幕
        var screens = Screen.AllScreens;
        foreach (var screen in screens)
        {
            Console.WriteLine($"Screen: {screen.DeviceName}, Bounds: {screen.Bounds}");
        }
    }

    public void Dispose()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
    }
}
