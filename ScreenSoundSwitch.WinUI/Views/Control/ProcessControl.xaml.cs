using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.ViewModels;
using SoundSwitch.Audio.Manager;
using SoundSwitch.Audio.Manager.Interop.Com.User;
using SoundSwitch.Audio.Manager.Interop.Enum;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UserControl = Microsoft.UI.Xaml.Controls.UserControl;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    public sealed partial class ProcessControl : UserControl
    {
        private Process process;
        private Screen screen;
        private AudioSessionControl session;
        private bool sliderLock = false;
        private AudioSwitcher audioSwitcher;
        private ProcessControlViewModel viewModel;

        public ProcessControl(AudioSessionControl session)
        {
            this.InitializeComponent();
            viewModel = this.DataContext as ProcessControlViewModel;
            this.session = session;
            audioSwitcher = AudioSwitcher.Instance;
            SetProcess();
            this.PointerEntered += ProcessControl_PointerEntered;
            this.PointerExited += ProcessControl_PointerExited;
        }
        public int ProcessId
        {
            get { return process.Id; }
        }
        /// <summary>
        /// 该进程控件对应的进程所处的屏幕是否发生改变，如发生改变则更新screen属性
        /// </summary>
        /// <param name="screen"></param>
        /// <returns>screen==this.screen -> false</returns>
        public bool IsScreenChange(Screen screen)
        {
            if (this.screen.Equals(screen))
            {
                return false;
            }
            this.screen = screen;
            return true;
        }
        public void ChangeSimpleVolumeLevel(float level)
        {
            session.SimpleAudioVolume.Volume += level;
        }
        public void ChangeAudioDevice(MMDevice mMDevice)
        {
            audioSwitcher.SwitchProcessTo(mMDevice.ID, ERole.ERole_enum_count, EDataFlow.eRender, (uint)ProcessId);//ERole_enum_count，将该设备分配所有角色任务
        }

        public void ApplyScreenPositionChannelBalance(Screen targetScreen, MMDevice targetDevice, IntPtr hwnd)
        {
            if (targetDevice == null || targetDevice.AudioEndpointVolume == null)
            {
                DebugLogStore.Add($"Skip channel balance for process {ProcessId}: target device is null.");
                return;
            }

            var screens = Screen.AllScreens;
            if (screens.Length <= 1)
            {
                DebugLogStore.Add($"Skip channel balance for process {ProcessId}: only one screen detected.");
                return;
            }

            if (hwnd == IntPtr.Zero || !User32.NativeMethods.GetWindowRect(User32.NativeMethods.HWND.Cast(hwnd), out var windowRect))
            {
                DebugLogStore.Add($"Skip channel balance for process {ProcessId}: failed to get window rect.");
                return;
            }

            var virtualScreen = SystemInformation.VirtualScreen;
            var minX = virtualScreen.Left;
            var width = virtualScreen.Width;
            if (width <= 0)
            {
                DebugLogStore.Add($"Skip channel balance for process {ProcessId}: invalid screen width range.");
                return;
            }

            // Microsoft docs (GetWindowRect/RECT): left/top/right/bottom are edges in screen coordinates,
            // right/bottom are exclusive. Use edge + width/2 to get center X.
            var centerX = windowRect.Left + ((windowRect.Right - windowRect.Left) / 2.0);

            var normalized = (centerX - minX) / width; // 0..1
            normalized = Math.Max(0.0, Math.Min(1.0, normalized));

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var strength = 20.0;
            if (localSettings.Values["ScreenPositionChannelBalanceStrength"] != null)
            {
                strength = Convert.ToDouble(localSettings.Values["ScreenPositionChannelBalanceStrength"]);
            }

            // 以 50 为基准，窗口越靠左提高左声道，越靠右提高右声道。
            var bias = (normalized - 0.5) * 2.0; // -1..1
            var left = Clamp(50.0 - bias * strength);
            var right = Clamp(50.0 + bias * strength);

            if (targetDevice.AudioEndpointVolume.Channels.Count < 2)
            {
                DebugLogStore.Add($"Skip channel balance for process {ProcessId}: device {targetDevice.FriendlyName} has less than 2 channels.");
                return;
            }

            targetDevice.AudioEndpointVolume.Channels[0].VolumeLevelScalar = (float)(left / 100.0);
            targetDevice.AudioEndpointVolume.Channels[1].VolumeLevelScalar = (float)(right / 100.0);
            targetDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(Math.Max(left, right) / 100.0);
            DebugLogStore.Add($"Channel balance applied for process {ProcessId} on {targetScreen.DeviceName}: L={left:F0}, R={right:F0}, strength={strength:F0}, device={targetDevice.FriendlyName}");
        }

        private static double Clamp(double value)
        {
            if (value < 0) return 0;
            if (value > 100) return 100;
            return value;
        }

        private void SetProcess()
        {
            process = Process.GetProcessById((int)session.GetProcessID);
            viewModel.SetProcessName(process.ProcessName);
            screen = Screen.FromHandle(process.Handle);
            SimpleVolumeSlider.Value = session.SimpleAudioVolume.Volume * 100;

            try
            {
                var icon = Icon.ExtractAssociatedIcon(process.MainModule?.FileName);
                if (icon != null)
                {
                    // 将Icon转换为BitmapImage
                    BitmapImage bitmapImage = ConvertIconToBitmapImage(icon);
                    viewModel.SetImage(bitmapImage);
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Fallback for when a 32-bit process tries to access a 64-bit process module
                // Ignore the exception, the process will just not have an icon
            }
        }
        private BitmapImage ConvertIconToBitmapImage(Icon icon)
        {
            using (var stream = new MemoryStream())
            {
                icon.ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream.AsRandomAccessStream());
                return bitmapImage;
            }
        }

        private void SimpleVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (sliderLock)
            {
                return;
            }
            session.SimpleAudioVolume.Volume = (float)(e.NewValue / 100);
        }

        private void ProcessControl_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var border = this.FindName("ProcessRowBorder") as Microsoft.UI.Xaml.Controls.Border;
            if (border != null)
            {
                border.Background = (Microsoft.UI.Xaml.Media.Brush)App.Current.Resources["SubtleFillColorSecondaryBrush"];
            }
        }

        private void ProcessControl_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var border = this.FindName("ProcessRowBorder") as Microsoft.UI.Xaml.Controls.Border;
            if (border != null)
            {
                border.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            }
        }
    }
}
