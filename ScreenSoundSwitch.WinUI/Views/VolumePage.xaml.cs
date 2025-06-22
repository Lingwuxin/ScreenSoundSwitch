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
        /// �����۽������Ƿ����仯
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
        /// ����Ctrl+Shift+�����֣�������ǰ�۽����ڵ�����
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
        public void UpdataForegroundProcess(uint processId)//���µ�ǰ�۽����ڵ�pid
        {
            foreach (var audioDeviceControl in DevicesStackPanel.Children.OfType<AudioDeviceControl>())
            {
                foreach (var processControl in audioDeviceControl._ProcessStackPanel.Children.OfType<ProcessControl>())
                {
                    Debug.WriteLine($"processControl.ProcessId={processControl.ProcessId} ForegroundProcessId={processId}");
                    if (processId == processControl.ProcessId)//processControl�ǵ�ǰ�Ѿ�����Ƶ�豸����seesion�Ľ��̿ؼ�
                    {
                        foregroundProcessControl = processControl;//��ǰ�۽��Ľ�����
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
        /// �����������Ӧ�Ĵ��ڵ��ƶ��¼����жϽ����Ƿ�ӵ�ǰ��������ʾ�����ƶ�����һ����ʾ���ϣ����������processModel���ṩ���豸������л��ý��̵Ĳ����豸
        /// </summary>
        /// <param name="hwnd">��ǰ���ڱ��ƶ��ı��۽��Ĵ��ڵľ��</param>
        /// <param name="pid">��Ӧ���ڵĽ���id</param>
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

            // ��� previousDevices �Ѵ��ڲ����� currentDevices ��ͬ����ֱ�ӷ���
            if (previousDevices != null && previousDevices.Count == currentDevices.Count &&
                !previousDevices.Where((t, i) => !t.ID.Equals(currentDevices[i].ID)).Any())
            {
                return;
            }

            // ���� previousDevices
            previousDevices = currentDevices;

            DevicesStackPanel.Children.Clear(); // ���֮ǰ������
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
