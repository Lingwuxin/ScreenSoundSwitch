using Microsoft.Win32;
using System;

namespace ScreenSoundSwitch.WinUI.Data
{
    public static class StartupManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "ScreenSoundSwitch";

        public static bool IsEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
            return key?.GetValue(AppName) != null;
        }

        public static void SetEnabled(bool enabled)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true) ?? Registry.CurrentUser.CreateSubKey(RunKeyPath);
            if (key == null)
            {
                return;
            }

            if (!enabled)
            {
                key.DeleteValue(AppName, false);
                return;
            }

            var processPath = Environment.ProcessPath;
            if (string.IsNullOrWhiteSpace(processPath))
            {
                return;
            }

            key.SetValue(AppName, $"\"{processPath}\"");
        }
    }
}
