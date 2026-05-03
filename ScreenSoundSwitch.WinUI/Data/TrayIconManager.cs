using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ScreenSoundSwitch.WinUI.Data
{
    public sealed class TrayIconManager : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private Icon? _ownedIcon;
        private bool _disposed;

        public event Action? OpenRequested;
        public event Action? HideRequested;
        public event Action? ExitRequested;

        public void Enable()
        {
            if (_disposed || _notifyIcon != null)
            {
                return;
            }

            var menu = new ContextMenuStrip();
            menu.Items.Add("打开", null, (_, _) => OpenRequested?.Invoke());
            menu.Items.Add("隐藏窗口", null, (_, _) => HideRequested?.Invoke());
            menu.Items.Add("退出", null, (_, _) => ExitRequested?.Invoke());

            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = "ScreenSoundSwitch",
                Icon = _ownedIcon = GetTrayIcon(),
                ContextMenuStrip = menu
            };

            _notifyIcon.DoubleClick += (_, _) => OpenRequested?.Invoke();
        }

        private static Icon GetTrayIcon()
        {
            try
            {
                var trayIconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "TrayIcon.ico");
                if (File.Exists(trayIconPath))
                {
                    return new Icon(trayIconPath);
                }

                var processPath = Environment.ProcessPath;
                if (!string.IsNullOrWhiteSpace(processPath) && File.Exists(processPath))
                {
                    var icon = Icon.ExtractAssociatedIcon(processPath);
                    if (icon != null)
                    {
                        return icon;
                    }
                }
            }
            catch
            {
                // ignore and fallback
            }

            return SystemIcons.Application;
        }

        public void Disable()
        {
            if (_notifyIcon == null)
            {
                return;
            }

            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
            _ownedIcon?.Dispose();
            _ownedIcon = null;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Disable();
        }
    }
}
