using Microsoft.UI.Xaml.Controls;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Application = Microsoft.UI.Xaml.Application;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// 进程音量显示管理页面
    /// </summary>
    public sealed partial class ProcessPage : Page
    {
        private ProcessControl foregroundProcessControl;
        private ScreenToAudioDevice screenToAudioDevice;
        private MMDeviceViewModel MMDeviceViewModel => ((App)Application.Current).MMDeviceViewModel;
        private MMDeviceCollection mMDevices;
        public ProcessPage()
        {
            this.InitializeComponent();
            screenToAudioDevice = ScreenToAudioDevice.Instance;
        }
        public void UpdateProcessBySeesion()
        {
            if (mMDevices == null)
            {
                mMDevices = MMDeviceViewModel.ShareDate;
            }
            ProcessStackPanel.Children.Clear();
            foreach (var device in mMDevices)
            {
                var textDeviceName = new TextBlock();
                textDeviceName.Text = device.FriendlyName;
                ProcessStackPanel.Children.Add(textDeviceName);
                device.AudioSessionManager.RefreshSessions();
                var sessions = device.AudioSessionManager.Sessions;
                for (int i = 0; i < sessions.Count; i++)
                {
                    if (sessions[i].IsSystemSoundsSession)
                    {
                        continue;
                    }
                    ProcessStackPanel.Children.Add(new ProcessControl(sessions[i]));
                }

            }
        }

        public void UpdataForegroundProcess(uint processId)//更新当前聚焦窗口的pid
        {
            foreach (var processControl in ProcessStackPanel.Children.OfType<ProcessControl>())
            {
                Debug.WriteLine($"processControl.ProcessId={processControl.ProcessId} ForegroundProcessId={processId}");
                if (processId == processControl.ProcessId)//processControl是当前已经与音频设备建立seesion的进程控件
                {
                    foregroundProcessControl = processControl;//当前聚焦的进程与
                    Debug.WriteLine(processControl.ProcessId);
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
            if(processId!= foregroundProcessControl.ProcessId)
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
    }
}
