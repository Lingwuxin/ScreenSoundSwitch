//v0.0.1
using SoundSwitch.Audio.Manager;
using SoundSwitch.Audio.Manager.Interop.Enum;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ScreenSoundSwitch.UI;
using Windows.Networking.Sockets;
using Microsoft.Win32;
namespace ScreenSoundSwitch
{
    public partial class MainForm : Form
    {
        AudioSwitcher audioSwitcher = AudioSwitcher.Instance;
        MMDeviceCollection deviceCollection;
        AudioDeviceManger audioDeviceEnumerator = new AudioDeviceManger();
        Screen[] screens;
        Dictionary<uint, ProcessInfo> processInfoDict = new Dictionary<uint, ProcessInfo>();
        Dictionary<string, MMDevice?> deviceInfoDict = new Dictionary<string, MMDevice?>();
        Dictionary<int, string> screenIndexToAudioDeviceName = new Dictionary<int, string>();
        Dictionary<string,Screen> deviceNameToScreen = new Dictionary<string, Screen>();
        Dictionary<int, IntPtr> processIdHookDict = new Dictionary<int, IntPtr>();
        DeviceControl deviceSelectControl = new DeviceControl();
        ForegroundProcessWatcher watcher;
        bool isBounded = false;
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
        //����û��ؼ�
        private void AddUserControl()
        {
            // ��ȡ������ʾ�������Ƕ�Ӧ�Ĳ����豸
            deviceCollection = audioDeviceEnumerator.GetDevices();
            //����Ƶ�����豸��ӵ��ֵ��в����õ�VolumeControl
            foreach (var device in deviceCollection)
            {
                deviceInfoDict.Add(device.FriendlyName, device);
                Debug.WriteLine(device.FriendlyName + "�����");
                setVolumeControl(device);
                deviceSelectControl.comBoxAudio.Items.Add(device.FriendlyName);
            }
            screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                deviceSelectControl.comBoxScreen.Items.Add(screens[i].DeviceName);
            }
            deviceSelectControl.selectButton.Click += bound_button_Click;
            

            selectDevicePage.Controls.Add(deviceSelectControl);
            tabControl.SelectedIndexChanged += SelectPageChange;
        }
        private void setVolumeControl(MMDevice? mMDevice)
        {
            volumepPageTableLayout.ColumnCount = deviceCollection.Count;
            volumepPageTableLayout.RowCount = 1;
            int count = 0;
            foreach (var device in deviceCollection)
            {
                VolumeControl volumeControl = new VolumeControl();
                volumeControl.setDevice(mMDevice);
                volumepPageTableLayout.Controls.Add(volumeControl, count, 0);
                count++;
            }
        }
        private void SelectPageChange(object? sender, EventArgs e)
        {
            switch(tabControl.SelectedIndex)
            {
                case 0:
                    if (IsSelectConrtolChanged())
                    {
                        UpdateSelectControl();
                    }
                    break;
                case 1:
                    if (IsAudioDeviceChanged())
                    {
                        UpdateVolumeControl();
                    }
                    break;
            }
        }
        private bool IsSelectConrtolChanged()
        {

            if (IsAudioDeviceChanged() || IsScreenChanged())
            {
                return true;
            }
            return false;
        }
        private bool IsScreenChanged()
        {
            //��Ļ�豸��Ϣ�Ƿ����
            Screen[] screens_new = Screen.AllScreens;
            if (screens.Length == screens_new.Length)
            {
                for (int i = 0; i < screens_new.Length; i++)
                {
                    if (!screens_new[i].DeviceName.Equals(screens[i].DeviceName))
                    {
                        Debug.WriteLine("IsScreenChanged");
                        return true;
                    }
                }
            }
            return false;
        }
        //�����豸��Ϣ�Ƿ����
        private bool IsAudioDeviceChanged()
        {
            MMDevice[] audios= audioDeviceEnumerator.GetDevices().ToArray();
            foreach(MMDevice mMDevice in audios)
            {
                if (!deviceInfoDict.ContainsKey(mMDevice.FriendlyName))
                {
                    Debug.WriteLine("IsAudioDeviceChanged");
                    return true;
                }
            }
            return false;
        }
        private void UpdateSelectControl()
        {
            deviceSelectControl.comBoxAudio.Items.Clear();
            deviceCollection = audioDeviceEnumerator.GetDevices();
            foreach (var device in deviceCollection)
            {
                deviceInfoDict[device.FriendlyName] = device;
                deviceSelectControl.comBoxAudio.Items.Add(device.FriendlyName);
            }
        }
        private void UpdateVolumeControl()
        {
            Debug.WriteLine("=======================UpdateVolumeControl======================");
            Debug.WriteLine("δ���");
            Debug.WriteLine("=======================UpdateVolumeControl======================");

        }
        //��Json�ļ��ж�ȡ����
        private void LoadConfig()
        {
            if(appConfig.ReadDeviceConfig())
            {
                foreach (var deviceConfig in appConfig.GetDevicesConfig())
                {
                    Debug.WriteLine(deviceConfig.FriendlyName);
                    //������豸����ʹ�ã�����Ӹ��豸����Ϣ
                    if (deviceInfoDict.ContainsKey(deviceConfig.FriendlyName))
                    {
                        Debug.WriteLine("Read " + deviceConfig.FriendlyName + " mesg");
                        screenIndexToAudioDeviceName[deviceConfig.MonitorIndex] = deviceConfig.FriendlyName;
                        deviceSelectControl.updateList(screens[deviceConfig.MonitorIndex].DeviceName, deviceConfig.FriendlyName);
                        deviceNameToScreen[deviceConfig.FriendlyName] = screens[deviceConfig.MonitorIndex];
                        isBounded = true;
                    }
                }
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            AddUserControl();
            LoadConfig();
            //��ӽ��̼�����,��̭��ʱ��δʹ�õĽ���
            Thread timerThread = new Thread(LookTimer);
            timerThread.Start();
            watcher = new ForegroundProcessWatcher();
            // �����¼�
            watcher.ForegroundProcessChanged += Watcher_ForegroundProcessChanged;
            watcher.HookNeedDisable += UnWinHook;

        }
        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Icon;
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem restoreMenuItem = new ToolStripMenuItem("��");
            restoreMenuItem.Click += RestoreMenuItem_Click;
            contextMenu.Items.Add(restoreMenuItem);
            ToolStripMenuItem autostartMenuItem = new ToolStripMenuItem("��������");
            autostartMenuItem.Click += AutostartMenuItem_Click;
            contextMenu.Items.Add(autostartMenuItem);
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("�˳�");
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);
            notifyIcon.ContextMenuStrip = contextMenu;
            appConfig.ReadWinformConfig();
            autoStartEnabled = appConfig.IsAutoStart();
            autostartMenuItem.Checked = autoStartEnabled;
        }
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
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

        //����ʹ��Close()
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
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

        }
        private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            appConfig.WriteConfig();
            foreach (var hook in processIdHookDict.Values)
            {
                UnWinHook(hook);
            }
            notifyIcon.Dispose();
        }

        private void bound_button_Click(object? sender, EventArgs e)
        {
            if (deviceSelectControl.comBoxScreen.SelectedItem != null && deviceSelectControl.comBoxAudio.SelectedItem != null)
            {
                int screenIndex= deviceSelectControl.comBoxScreen.SelectedIndex;
                string deviceName = (string)deviceSelectControl.comBoxAudio.SelectedItem;
                deviceNameToScreen[deviceName] = screens[screenIndex];
                screenIndexToAudioDeviceName[screenIndex] = deviceName;
                deviceSelectControl.updateList((string)deviceSelectControl.comBoxScreen.SelectedItem, deviceName);
                float deviceVolume = deviceInfoDict[deviceName].AudioEndpointVolume.MasterVolumeLevelScalar*10;
                appConfig.AddDeviceConfig(deviceName, screenIndex,(int)deviceVolume);
                isBounded = true;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]

        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        // ����ö�ٴ��ڵ�ί��
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);



        public static string GetWindowTitle(IntPtr hWnd)
        {
            // ��ȡ���ڱ���ĳ���
            int length = GetWindowTextLength(hWnd);
            if (length == 0)
                return string.Empty;

            // �����ڴ滺�������洢���ڱ���
            StringBuilder stringBuilder = new StringBuilder(length + 1);

            // ��ȡ���ڱ���
            GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);

            return stringBuilder.ToString();
        }

        private int ClosetMonitor(IntPtr mainWindowHandle)
        {
            int closestMonitorIndex = -1;
            int closestDistance = int.MaxValue;
            RECT windowRect = new RECT();
            GetWindowRect(mainWindowHandle, ref windowRect);
            for (int i = 0; i < screens.Length; i++)
            {
                Rectangle monitorBounds = screens[i].Bounds;
                int distance = Math.Abs(windowRect.Left - monitorBounds.Left) +
                                Math.Abs(windowRect.Top - monitorBounds.Top);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMonitorIndex = i;
                }
            }
            return closestMonitorIndex;
        }
        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
    IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Debug.WriteLine("WinEventProc");
            GetWindowThreadProcessId(hWnd, out uint processId);

            if (processInfoDict.ContainsKey(processId))
            {
                processInfoDict[processId].process = processInfoDict[processId].process;
                processInfoDict[processId].process.Refresh();
            }
            else
            {
                Debug.WriteLine("not found process");
                return;
            }
            //Process process = Process.GetProcessById((int)processId);
            IntPtr mainWindowHandle = processInfoDict[processId].process.MainWindowHandle;

            if (mainWindowHandle != hWnd || mainWindowHandle == IntPtr.Zero)
            {
                Debug.WriteLine("mainWindowHandle != hWnd");
                return;
            }
            // ��ȡ�������ڵ���Ļ

            IntPtr hMonitor = MonitorFromWindow(mainWindowHandle, 0x00000002);
            if (hMonitor == IntPtr.Zero)
            {
                Debug.WriteLine("hMonitor == IntPtr.Zero");
                return;
            }
            // ��ȡ��Ļ��Ϣ
            MONITORINFOEX monitorInfo = new MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            GetMonitorInfo(hMonitor, ref monitorInfo);

            int closestMonitorIndex= ClosetMonitor(mainWindowHandle);

            if (processInfoDict[processId].MonitorIndex.Equals(closestMonitorIndex))
            {
                Debug.WriteLine("same screen");
                return;
            }
            string winTitle = GetWindowTitle(mainWindowHandle);
            processInfoDict[processId].MonitorIndex = closestMonitorIndex;


            if (!screenIndexToAudioDeviceName.ContainsKey(processInfoDict[processId].MonitorIndex))
            {
                Debug.WriteLine("No screen selected for " + processInfoDict[processId].MonitorIndex);
                return;
            }
            string screenIndex = screenIndexToAudioDeviceName[processInfoDict[processId].MonitorIndex];
            if (deviceInfoDict[screenIndex] == null)
            {
                Debug.WriteLine("No audio device selected " + processInfoDict[processId].MonitorIndex);
                return;
            }
            MMDevice? device = deviceInfoDict[screenIndex];
            if (device != null)
            {
                EDataFlow eDataFlow = new EDataFlow();
                ERole eRole = new ERole();
                //textBox1.Text += screenIndexToAudioDevice[processInfo.MonitorIndex] + processId;
                try
                {
                    audioSwitcher.SwitchProcessTo(device.ID, eRole, eDataFlow, processId);

                    Debug.WriteLine("Switching audio process " + processId + " title is " + winTitle + " on Screen: " + processInfoDict[processId].MonitorIndex);
                }
                catch (Exception ex)
                {
                    // �����쳣�������¼��־������ʾ������Ϣ
                    MessageBox.Show("An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            // �����Ļ���
            //textBox1.Text += "Process " + processId + " title is " + winTitle + " on Screen: " + processInfo.MonitorIndex + "\r\n";
        }
        // ����Windows API����
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);


        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);


        //�������̴����Ƿ��ƶ�
        void CreateWinHook(int processid)
        {
            try
            {
                IntPtr hook = SetWinEventHook(0x000B, 0x000B, IntPtr.Zero, WinEventProc, (uint)processid, 0, 0);
                processIdHookDict[processid] = hook;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //textBox1.Text += $"����{GetWindowTitle(Process.GetProcessById(processid).MainWindowHandle)}{processid}'s hook\r\n";
        }

        void UnWinHook(IntPtr hook)
        {
            if (hook != IntPtr.Zero)
            {
                Invoke(new Action(() =>
                {
                    if (UnhookWinEvent(hook))
                    {   //ֻ���ڴ���h ook�Ľ�����ж��hook
                        Debug.WriteLine("Unhook success��");
                    }
                    else
                    {
                        Debug.WriteLine("unhook on thread" + Thread.CurrentThread.ManagedThreadId);
                        Debug.WriteLine("Unhook failed��");
                    }
                }
                ));
            }
        }

        private void Watcher_ForegroundProcessChanged(int processId)
        {
            /*
            //�жϽ����Ƿ�ʹ����Ƶ�豸
            if (!IsUsingAudioDeviceByProcessId(processId))
            {
                //�����ʱ����û��ʹ����Ƶ�豸���ж��Ƿ��Ѿ���ӵ�processInfoDict��ж�ع���
                if(processIdHookDict.ContainsKey(processId))
                {
                    UnWinHook(processIdHookDict[processId]);
                }
                return;
            }*/
            //�жϽ����Ƿ��Ѿ���ӵ�processInfoDict
            if (processInfoDict.ContainsKey((uint)processId))
            {
                processInfoDict[(uint)processId].resetTime();
                return;
            }
            ProcessInfo processInfo = new ProcessInfo();
            processInfo.process = Process.GetProcessById(processId);
            processInfo.MonitorIndex=ClosetMonitor(processInfo.process.MainWindowHandle);
            processInfoDict[(uint)processId] = processInfo;
            Invoke(new Action(() =>
            {
                CreateWinHook(processId);
            }));

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
                        
                        processIdHookDict.Remove(processid);
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
