using System;

namespace ScreenSoundSwitch.WinUI.Data
{
    public static class TrayIconState
    {
        public static event Action<bool>? EnabledChanged;

        public static bool IsEnabled { get; private set; }

        public static void SetEnabled(bool enabled)
        {
            if (IsEnabled == enabled)
            {
                return;
            }

            IsEnabled = enabled;
            EnabledChanged?.Invoke(enabled);
        }
    }
}
