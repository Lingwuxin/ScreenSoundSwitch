using Microsoft.Win32;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.UI;
using SoundSwitch.Audio.Manager;
using SoundSwitch.Audio.Manager.Interop.Enum;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
namespace ScreenSoundSwitch
{
    public partial class MainForm : Form
    {
        AudioSwitcher audioSwitcher = AudioSwitcher.Instance;
        AudioDeviceManger audioDeviceManger = new AudioDeviceManger();
        MMDeviceCollection deviceCollection;
        Screen[] screens;
        Dictionary<uint, ProcessInfo> processInfoDict = new Dictionary<uint, ProcessInfo>();
        Dictionary<string, MMDevice?> deviceInfoDict = new Dictionary<string, MMDevice?>();
        Dictionary<int, string> screenIndexToAudioDeviceName = new Dictionary<int, string>();
        Dictionary<string, int> screenNameToScreenIndex = new Dictionary<string, int>();
        Dictionary<string, Screen> deviceNameToScreen = new Dictionary<string, Screen>();
        DeviceControl deviceSelectControl = new DeviceControl();
        private WindowMonitor _windowMonitor;
        bool isBounded = false;//设置音频设备是否已经与显示器绑定
        private NotifyIcon notifyIcon;
        private AppConfig appConfig = new AppConfig();
        private bool autoStartEnabled = false;
        private const string StartupRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AppName = "ScreenSoundSwitch";

        public MainForm()
        {
            InitializeComponent();
            InitializeNotifyIcon();
        }
        private void AddUserControl()
        {
            deviceCollection = audioDeviceManger.GetDevices();
            screens = Screen.AllScreens;
            UpdateVolumeControl();
            InitSelectControl();
            tabControl.SelectedIndexChanged += SelectPageChange;
        }
        private void InitSelectControl()
        {
            UpdateSelectControl();
            deviceSelectControl.selectButton.Click += bound_button_Click;
            selectDevicePage.Controls.Add(deviceSelectControl);
        }
        private void UpdateVolumeControl()
        {
            volumepPageTableLayout.Controls.Clear();
            volumepPageTableLayout.ColumnCount = deviceCollection.Count;
            volumepPageTableLayout.RowCount = 1;
            int count = 0;
            foreach (var device in deviceCollection)
            {
                VolumeControl volumeControl = new VolumeControl();
                volumeControl.setDevice(device);
                volumepPageTableLayout.Controls.Add(volumeControl, count, 0);
                count++;
            }
        }
        private void UpdateSelectControl()
        {

            deviceSelectControl.comBoxAudio.Items.Clear();
            deviceSelectControl.comBoxScreen.Items.Clear();
            foreach (var device in deviceCollection)
            {
                deviceInfoDict[device.FriendlyName] = device;
                deviceSelectControl.comBoxAudio.Items.Add(device.FriendlyName);
            }

            for (int i = 0; i < screens.Length; i++)
            {
                string deviceName = screens[i].DeviceName;
                deviceSelectControl.comBoxScreen.Items.Add(deviceName);
                screenNameToScreenIndex[deviceName] = i;
            }
        }
        private void SelectPageChange(object? sender, EventArgs e)
        {
            deviceCollection = audioDeviceManger.GetDevices();
            if (IsScreenChanged() || IsAudioDeviceChanged())
            {
                UpdateSelectControl();
                UpdateVolumeControl();
            }
        }
        private bool IsScreenChanged()
        {

            Screen[] new_screens = Screen.AllScreens;
            if (screens.Equals(new_screens))
            {
                return false;
            }
            else
            {
                Debug.WriteLine("IsScreenChanged");
                screens = new_screens;
                return true;
            }
        }
        //判断音频驱动是否在deviceInfoDict中
        private bool IsAudioDeviceChanged()
        {
            foreach (MMDevice mMDevice in deviceCollection)
            {
                if (!deviceInfoDict.ContainsKey(mMDevice.FriendlyName))
                {
                    Debug.WriteLine("IsAudioDeviceChanged");
                    return true;
                }
            }
            return false;
        }


        //通过json加载配置
        private void LoadConfig()
        {
            if (appConfig.ReadDeviceConfig())
            {
                foreach (var deviceConfig in appConfig.GetDevicesConfig())
                {
                    if (deviceInfoDict.ContainsKey(deviceConfig.FriendlyName) && screenNameToScreenIndex.ContainsKey(deviceConfig.MonitorName))
                    {
                        Debug.WriteLine("Read " + deviceConfig.FriendlyName + " mesg");
                        int monitorIndex = screenNameToScreenIndex[deviceConfig.MonitorName];
                        screenIndexToAudioDeviceName[monitorIndex] = deviceConfig.FriendlyName;
                        deviceSelectControl.updateList(deviceConfig.MonitorName, deviceConfig.FriendlyName);
                        deviceNameToScreen[deviceConfig.FriendlyName] = screens[monitorIndex];
                        isBounded = true;
                    }
                }
            }
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Icon;
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem restoreMenuItem = new ToolStripMenuItem("打开");
            restoreMenuItem.Click += RestoreMenuItem_Click;
            contextMenu.Items.Add(restoreMenuItem);
            ToolStripMenuItem autostartMenuItem = new ToolStripMenuItem("开机启动");
            autostartMenuItem.Click += AutostartMenuItem_Click;
            contextMenu.Items.Add(autostartMenuItem);
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("退出");
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);
            notifyIcon.ContextMenuStrip = contextMenu;
            appConfig.ReadWinformConfig();
            autoStartEnabled = appConfig.IsAutoStart();
            autostartMenuItem.Checked = autoStartEnabled;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            AddUserControl();
            LoadConfig();
        }
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (Visible)
            {
                Hide();
            }
            else
            {
                Show();
                WindowState = FormWindowState.Normal;
            }

        }
        private void AutostartMenuItem_Click(object sender, EventArgs e)
        {
            autoStartEnabled = !autoStartEnabled;
            ((ToolStripMenuItem)sender).Checked = autoStartEnabled;
            appConfig.SetAutoStart(autoStartEnabled);
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true))
            {
                if (autoStartEnabled)
                {
                    key.SetValue(AppName, Application.ExecutablePath);
                }
                else
                {
                    key.DeleteValue(AppName, false);
                }
            }

        }
        private void RestoreMenuItem_Click(object sender, EventArgs e)
        {
            NotifyIcon_DoubleClick(sender, e);
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            //设置计时器，超时不再监听线程
            //这是一个糟糕的设计，目前通过聚焦窗口来判断窗口是否移动，因此不需要监听大量的无关进程
            Thread timerThread = new Thread(LookTimer);
            timerThread.Start();
            _windowMonitor = new WindowMonitor();
            // 订阅事件
            _windowMonitor.ForegroundChanged += OnForegroundChanged;
            _windowMonitor.ForegroundWindowMoved += OnForegroundWindowMoved;
            if (isBounded)
            {
                Hide();
            }
        }
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                _windowMonitor.ForegroundChanged -= OnForegroundChanged;
                _windowMonitor.ForegroundWindowMoved -= OnForegroundWindowMoved;
                _windowMonitor.Dispose();
            }


        }
        private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            appConfig.WriteConfig();
            notifyIcon.Dispose();
        }
        private void bound_button_Click(object? sender, EventArgs e)
        {
            if (deviceSelectControl.comBoxScreen.SelectedItem != null && deviceSelectControl.comBoxAudio.SelectedItem != null)
            {
                int screenIndex = deviceSelectControl.comBoxScreen.SelectedIndex;
                string deviceName = (string)deviceSelectControl.comBoxAudio.SelectedItem;
                deviceNameToScreen[deviceName] = screens[screenIndex];
                screenIndexToAudioDeviceName[screenIndex] = deviceName;
                deviceSelectControl.updateList((string)deviceSelectControl.comBoxScreen.SelectedItem, deviceName);
                float deviceVolume = deviceInfoDict[deviceName].AudioEndpointVolume.MasterVolumeLevelScalar * 10;

                appConfig.AddDeviceConfig(deviceName, screens[screenIndex].DeviceName, (int)deviceVolume);
                isBounded = true;
            }
        }
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        public static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length == 0)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder(length + 1);

            GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);

            return stringBuilder.ToString();
        }
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        private void OnForegroundChanged(object? sender, WindowMonitor.Event e)
        {
            /*
            //通过进程与音频设备驱动之间的会话来判断是否在使用音频设备
            if (!IsUsingAudioDeviceByProcessId(processId))
            {
                if(processIdHookDict.ContainsKey(processId))
                {
                    UnWinHook(processIdHookDict[processId]);
                }
                return;
            }*/

            var processId = e.ProcessId;
            if (processInfoDict.ContainsKey(processId))
            {
                processInfoDict[processId].resetTime();
                return;
            }
            ProcessInfo processInfo = new ProcessInfo();
            processInfo.process = Process.GetProcessById((int)processId);
            processInfoDict[processId] = processInfo;
        }
        private void OnForegroundWindowMoved(object? sender, WindowMonitor.Event e)
        {
            var hWnd = e.Hwnd;
            var processId = e.ProcessId;
            Debug.WriteLine($"{e.ProcessName}：{e.ProcessId} is moved");
            if (processInfoDict.ContainsKey(processId))
            {
                processInfoDict[processId].process.Refresh();
            }
            else
            {
                ProcessInfo processInfo = new ProcessInfo();
                processInfo.process = Process.GetProcessById((int)processId);
                processInfoDict[processId] = processInfo;
                return;
            }
            //Process process = Process.GetProcessById((int)processId);
            IntPtr mainWindowHandle = processInfoDict[processId].process.MainWindowHandle;

            if (mainWindowHandle != hWnd || mainWindowHandle == IntPtr.Zero)
            {
                Debug.WriteLine("WinEventProc:mainWindowHandle != hWnd");
                return;
            }

            IntPtr hMonitor = MonitorFromWindow(mainWindowHandle, 0x00000002);
            if (hMonitor == IntPtr.Zero)
            {
                Debug.WriteLine("WinEventProc:hMonitor == IntPtr.Zero");
                return;
            }

            int lastMonitorIndex = processInfoDict[processId].lastMonitorIndex;

            if (processInfoDict[processId].MonitorIndex.Equals(lastMonitorIndex))
            {
                Debug.WriteLine("WinEventProc:same screen");
                return;
            }
            string winTitle = GetWindowTitle(mainWindowHandle);
            processInfoDict[processId].lastMonitorIndex = processInfoDict[processId].MonitorIndex;

            if (!screenIndexToAudioDeviceName.ContainsKey(processInfoDict[processId].MonitorIndex))
            {
                Debug.WriteLine("WinEventProc:No screen selected for " + processInfoDict[processId].MonitorIndex);
                return;
            }
            string screenIndex = screenIndexToAudioDeviceName[processInfoDict[processId].MonitorIndex];
            if (deviceInfoDict[screenIndex] == null)
            {
                Debug.WriteLine("WinEventProc:No audio device selected " + processInfoDict[processId].MonitorIndex);
                return;
            }
            MMDevice? device = deviceInfoDict[screenIndex];
            if (device != null)
            {
                EDataFlow eDataFlow = new EDataFlow();
                ERole eRole = new ERole();
                try
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            audioSwitcher.SwitchProcessTo(device.ID, eRole, eDataFlow, processId);
                        }));

                    }
                    else
                    {
                        audioSwitcher.SwitchProcessTo(device.ID, eRole, eDataFlow, processId);
                    }
                    Debug.WriteLine("WinEventProc:Switching audio process " + processId + " title is " + winTitle + " on Screen: " + processInfoDict[processId].MonitorIndex);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("WinEventProc:An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine(ex);
                }
            }
        }
        private void LookTimer()
        {
            while (true)
            {
                Thread.Sleep(1000);
                foreach (var key in processInfoDict.Keys)
                {
                    if (processInfoDict[key].isTimeOut())
                    {
                        int processid = processInfoDict[key].process.Id;

                        processInfoDict.Remove(key);
                    }
                    else
                    {
                        processInfoDict[key].countTime();
                    }
                }
            }
        }


    }
}
