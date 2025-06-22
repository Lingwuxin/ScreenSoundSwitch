using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Data;
using SoundSwitch.Audio.Manager;
using System;
using System.Diagnostics;
using System.Linq;
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
        public VolumePage()
        {
            this.InitializeComponent();
            screenToAudioDevice = ScreenToAudioDevice.Instance;
            windowMonitor = new WindowMonitor();
            windowMonitor.ForegroundChanged += WindowMonitor_ForegroundChanged;
            windowMonitor.MouseWheelScrolled += WindowMonitor_KeyIsDown;
            windowMonitor.ForegroundWindowMoved += WindowMonitor_ForegroundMoved;
        }
        private void WindowMonitor_ForegroundMoved(object sender, WindowMonitor.Event e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
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
            DispatcherQueue.TryEnqueue(() =>
            {
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
            DispatcherQueue.TryEnqueue(() =>
            {
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
                return;
            }
            if (delta > 0)
            {
                foregroundProcessControl.ChangeSimpleVolumeLevel(0.05f);
            }
            else
            {
                foregroundProcessControl.ChangeSimpleVolumeLevel(-0.05f);
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
            if (foregroundProcessControl == null)
            {
                Debug.WriteLine("foregroundProcessControl==null");
                return;
            }
            if (processId != foregroundProcessControl.ProcessId)
            {
                Debug.WriteLine($"Process:{processId} is not using Audio Devices ");
                return;
            }
            Screen screen = Screen.FromHandle(hwnd);

            if (screen == null) return;
            if (!foregroundProcessControl.IsScreenChange(screen)) return;
            if (screenToAudioDevice.ContainsKey(screen))
            {
                foregroundProcessControl.ChangeAudioDevice(screenToAudioDevice[screen]);
            }
        }

        private void UpdateDevices()
        {
            var currentDevices = AudioDeviceManager.Instance.Devices;

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
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateDevices();
        }
    }
}
