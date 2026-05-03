using System;

namespace ScreenSoundSwitch.WinUI.Data
{
    public static class DebugPageState
    {
        public static event Action<bool>? VisibilityChanged;

        public static bool IsEnabled { get; private set; }

        public static void SetEnabled(bool enabled)
        {
            if (IsEnabled == enabled)
            {
                return;
            }

            IsEnabled = enabled;
            VisibilityChanged?.Invoke(enabled);
        }
    }
}
