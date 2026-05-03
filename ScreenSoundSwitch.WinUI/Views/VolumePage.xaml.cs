using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Data;
using SoundSwitch.Audio.Manager;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.Storage;
using System.Windows.Forms;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VolumePage : Page
    {
        private MMDeviceCollection previousDevices;
        private WindowMonitor windowMonitor;
        private ProcessControl foregroundProcessControl;
        private ScreenToAudioDevice screenToAudioDevice;
        private DateTime _lastChannelBalanceApplyUtc = DateTime.MinValue;
        private const int ChannelBalanceThrottleMs = 80;
        private bool _enableLocationChangeTracking;
        private bool _isDisposed;
        public VolumePage()
        {
            this.InitializeComponent();
            screenToAudioDevice = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<ScreenToAudioDevice>(App.Current.Services);
            windowMonitor = new WindowMonitor();
            windowMonitor.ForegroundChanged += WindowMonitor_ForegroundChanged;
            windowMonitor.MouseWheelScrolled += WindowMonitor_KeyIsDown;
            windowMonitor.ForegroundWindowMoved += WindowMonitor_ForegroundMoved;

            var localSettings = ApplicationData.Current.LocalSettings;
            _enableLocationChangeTracking = localSettings.Values["EnableScreenPositionChannelBalance"] is bool enabled && enabled;
            windowMonitor.SetLocationChangeTracking(_enableLocationChangeTracking);
            ChannelBalanceState.SetEnabled(_enableLocationChangeTracking);
            ChannelBalanceState.EnabledChanged += ChannelBalanceState_EnabledChanged;
            if (App.m_window != null)
            {
                App.m_window.Closed += MainWindow_Closed;
            }

            DebugLogStore.Add("VolumePage initialized and window monitor started.");
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            ChannelBalanceState.EnabledChanged -= ChannelBalanceState_EnabledChanged;

            if (windowMonitor != null)
            {
                windowMonitor.ForegroundChanged -= WindowMonitor_ForegroundChanged;
                windowMonitor.MouseWheelScrolled -= WindowMonitor_KeyIsDown;
                windowMonitor.ForegroundWindowMoved -= WindowMonitor_ForegroundMoved;
                windowMonitor.Stop();
            }

            if (App.m_window != null)
            {
                App.m_window.Closed -= MainWindow_Closed;
            }
        }

        private void ChannelBalanceState_EnabledChanged(bool enabled)
        {
            _enableLocationChangeTracking = enabled;
            windowMonitor?.SetLocationChangeTracking(enabled);
            DebugLogStore.Add($"Channel-balance location tracking {(enabled ? "enabled" : "disabled")}.");
        }
        private void WindowMonitor_ForegroundMoved(object sender, WindowMonitor.Event e)
        {
            if (_isDisposed || DispatcherQueue == null)
            {
                return;
            }

            DebugLogStore.Add($"ForegroundWindowMoved event: pid={e.ProcessId}, hwnd={e.Hwnd}");
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_isDisposed)
                {
                    return;
                }

                var now = DateTime.UtcNow;
                if ((now - _lastChannelBalanceApplyUtc).TotalMilliseconds < ChannelBalanceThrottleMs)
                {
                    return;
                }

                _lastChannelBalanceApplyUtc = now;
                ForegroundMovedHandle(e.Hwnd, e.ProcessId);
            });
        }
        /// <summary>
        /// 监听聚焦窗口是否发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowMonitor_ForegroundChanged(object sender, WindowMonitor.Event e)
        {
            if (_isDisposed || DispatcherQueue == null)
            {
                return;
            }

            DebugLogStore.Add($"ForegroundChanged event: pid={e.ProcessId}");
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_isDisposed)
                {
                    return;
                }

                Debug.WriteLine("Into MindowMonitor_ForegroundChanged");
                UpdataForegroundProcess(e.ProcessId);
            });

        }

        /// <summary>
        /// 监听Ctrl+Shift+鼠标滚轮，调整当前聚焦窗口的音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowMonitor_KeyIsDown(object sender, WindowMonitor.MouseWheelEventArgs e)
        {
            if (_isDisposed || DispatcherQueue == null)
            {
                return;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                if (_isDisposed)
                {
                    return;
                }

                Debug.WriteLine(e.Delta);
                UpdataForegroundVolume(e.Delta);
            });
        }
        public void UpdataForegroundProcess(uint processId)//更新当前聚焦窗口的pid
        {
            foreach (var audioDeviceControl in DevicesStackPanel.Children.OfType<AudioDeviceControl>())
            {
                foreach (var processControl in audioDeviceControl._ProcessStackPanel.Children.OfType<ProcessControl>())
                {
                    Debug.WriteLine($"processControl.ProcessId={processControl.ProcessId} ForegroundProcessId={processId}");
                    if (processId == processControl.ProcessId)//processControl是当前已经与音频设备建立seesion的进程控件
                    {
                        foregroundProcessControl = processControl;//当前聚焦的进程与
                        Debug.WriteLine(processControl.ProcessId);
                    }

                }
            }

        }
        public void UpdataForegroundVolume(int delta)
        {
            if (foregroundProcessControl == null)
            {
                VolumeStatusInfoBar.Severity = InfoBarSeverity.Informational;
                VolumeStatusInfoBar.Message = "未找到当前前台进程会话，无法调节音量。";
                DebugLogStore.Add("Foreground process session not found for volume adjustment.");
                return;
            }
            if (delta > 0)
            {
                foregroundProcessControl.ChangeSimpleVolumeLevel(0.05f);
                VolumeStatusInfoBar.Severity = InfoBarSeverity.Success;
                VolumeStatusInfoBar.Message = $"进程 {foregroundProcessControl.ProcessId} 音量已增加。";
                DebugLogStore.Add($"Process {foregroundProcessControl.ProcessId} volume increased.");
            }
            else
            {
                foregroundProcessControl.ChangeSimpleVolumeLevel(-0.05f);
                VolumeStatusInfoBar.Severity = InfoBarSeverity.Success;
                VolumeStatusInfoBar.Message = $"进程 {foregroundProcessControl.ProcessId} 音量已降低。";
                DebugLogStore.Add($"Process {foregroundProcessControl.ProcessId} volume decreased.");
            }
        }
        /// <summary>
        /// 处理进程所对应的窗口的移动事件，判断进程是否从当前所处的显示器上移动到另一个显示器上，如是则根据processModel中提供的设备组合来切换该进程的播放设备
        /// </summary>
        /// <param name="hwnd">当前正在被移动的被聚焦的窗口的句柄</param>
        /// <param name="pid">对应窗口的进程id</param>
        public void ForegroundMovedHandle(IntPtr hwnd, uint processId)
        {
            Debug.WriteLine($"Into ForegroundMovedHandle: hwnd={hwnd},pid={processId}");
            if (hwnd == IntPtr.Zero) return;

            // 每次发生移动时重新去寻找对应进程的 ProcessControl
            ProcessControl targetProcessControl = null;
            foreach (var audioDeviceControl in DevicesStackPanel.Children.OfType<AudioDeviceControl>())
            {
                foreach (var processControl in audioDeviceControl._ProcessStackPanel.Children.OfType<ProcessControl>())
                {
                    if (processId == processControl.ProcessId)
                    {
                        targetProcessControl = processControl;
                        break;
                    }
                }
                if (targetProcessControl != null) break;
            }

            if (targetProcessControl == null)
            {
                Debug.WriteLine($"targetProcessControl==null for pid={processId}");
                VolumeStatusInfoBar.Severity = InfoBarSeverity.Warning;
                VolumeStatusInfoBar.Message = $"进程 {processId} 尚未建立可切换的音频会话。";
                DebugLogStore.Add($"Process {processId} has no switchable audio session.");
                return;
            }
            DebugLogStore.Add($"Matched ProcessControl for pid={processId}.");

            Screen screen = Screen.FromHandle(hwnd);

            if (screen == null)
            {
                DebugLogStore.Add($"Failed to resolve screen from hwnd={hwnd} for pid={processId}.");
                return;
            }
            DebugLogStore.Add($"Resolved screen for pid={processId}: {screen.DeviceName}");

            var localSettings = ApplicationData.Current.LocalSettings;
            var channelBalanceEnabled = localSettings.Values["EnableScreenPositionChannelBalance"] is bool enabled && enabled;
            var screenChanged = targetProcessControl.IsScreenChange(screen);

            // 声道平衡功能开启时，即使屏幕未变化也要根据窗口位置持续更新。
            if (!screenChanged && !channelBalanceEnabled)
            {
                DebugLogStore.Add($"Screen unchanged for pid={processId}, skip switching.");
                return;
            }

            if (screenToAudioDevice.TryGetDevice(screen, out var targetDevice))
            {
                if (screenChanged)
                {
                    targetProcessControl.ChangeAudioDevice(targetDevice);
                    DebugLogStore.Add($"SwitchProcessTo requested: pid={processId}, device={targetDevice.FriendlyName}, screen={screen.DeviceName}");
                }

                if (channelBalanceEnabled)
                {
                    targetProcessControl.ApplyScreenPositionChannelBalance(screen, targetDevice, hwnd);
                    UpdateDeviceChannelSliders(targetDevice);
                }
                else
                {
                    DebugLogStore.Add($"Channel balance feature disabled for pid={processId}.");
                }

                VolumeStatusInfoBar.Severity = InfoBarSeverity.Success;
                VolumeStatusInfoBar.Message = screenChanged
                    ? $"进程 {processId} 已切换到屏幕 {screen.DeviceName} 绑定设备。"
                    : $"进程 {processId} 已按窗口位置更新左右声道。";
                DebugLogStore.Add(screenChanged
                    ? $"Process {processId} switched to device mapped for {screen.DeviceName}."
                    : $"Process {processId} channel balance updated by window position on {screen.DeviceName}.");
            }
            else
            {
                VolumeStatusInfoBar.Severity = InfoBarSeverity.Warning;
                VolumeStatusInfoBar.Message = $"屏幕 {screen.DeviceName} 未绑定播放设备。";
                DebugLogStore.Add($"No playback device mapping found for screen {screen.DeviceName}.");
            }
        }

        private void UpdateDeviceChannelSliders(MMDevice targetDevice)
        {
            foreach (var audioDeviceControl in DevicesStackPanel.Children.OfType<AudioDeviceControl>())
            {
                if (audioDeviceControl.DeviceId == targetDevice.ID)
                {
                    audioDeviceControl.UpdateChannelSlidersFromDevice();
                    break;
                }
            }
        }

        private void UpdateDevices()
        {
            var audioDeviceManager = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AudioDeviceManager>(App.Current.Services);
            var currentDevices = audioDeviceManager.Devices;

            // 如果 previousDevices 已存在并且与 currentDevices 相同，就直接返回
            if (previousDevices != null && previousDevices.Count == currentDevices.Count &&
                !previousDevices.Where((t, i) => !t.ID.Equals(currentDevices[i].ID)).Any())
            {
                return;
            }

            // 更新 previousDevices
            previousDevices = currentDevices;

            DevicesStackPanel.Children.Clear(); // 清空之前的内容
            foreach (var device in currentDevices)
            {
                AudioDeviceControl audioDeviceControl = new AudioDeviceControl(device);

                DevicesStackPanel.Children.Add(audioDeviceControl);
            }

            VolumeStatusInfoBar.Severity = InfoBarSeverity.Informational;
            VolumeStatusInfoBar.Message = $"已加载 {currentDevices.Count} 个输出设备。";
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateDevices();
        }
    }
}
