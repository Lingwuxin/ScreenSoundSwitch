//v0.0.1
using SoundSwitch.Audio.Manager;
using SoundSwitch.Audio.Manager.Interop.Enum;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ScreenSoundSwitch.UI;
using Microsoft.Win32;
namespace ScreenSoundSwitch
{
    public partial class MainForm : Form
    {
        AudioSwitcher audioSwitcher = AudioSwitcher.Instance;
        MMDeviceCollection deviceCollection;
        AudioDeviceManger audioDeviceManger = new AudioDeviceManger();
        Screen[] screens;
        Dictionary<uint, ProcessInfo> processInfoDict = new Dictionary<uint, ProcessInfo>();
        Dictionary<string, MMDevice?> deviceInfoDict = new Dictionary<string, MMDevice?>();
        Dictionary<int, string> screenIndexToAudioDeviceName = new Dictionary<int, string>();
        Dictionary<string,int> screenNameToScreenIndex = new Dictionary<string,int>();
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
        //添加用户控件
        private void AddUserControl()
        {
            // 获取所有显示器和它们对应的播放设备
            deviceCollection = audioDeviceManger.GetDevices();
            //将音频播放设备添加到字典中并设置到VolumeControl
            foreach (var device in deviceCollection)
            {
                deviceInfoDict.Add(device.FriendlyName, device);
                Debug.WriteLine(device.FriendlyName + "已添加");
                setVolumeControl(device);
                deviceSelectControl.comBoxAudio.Items.Add(device.FriendlyName);
            }
            screens = Screen.AllScreens;
            screenNameToScreenIndex.Clear();
            for (int i = 0; i < screens.Length; i++)
            {
                string deviceName = screens[i].DeviceName;
                deviceSelectControl.comBoxScreen.Items.Add(deviceName);
                screenNameToScreenIndex[deviceName]= i;
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
            //屏幕设备信息是否更改
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
        //播放设备信息是否更改
        private bool IsAudioDeviceChanged()
        {
            MMDevice[] audios= audioDeviceManger.GetDevices().ToArray();
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
            deviceCollection = audioDeviceManger.GetDevices();
            foreach (var device in deviceCollection)
            {
                deviceInfoDict[device.FriendlyName] = device;
                deviceSelectControl.comBoxAudio.Items.Add(device.FriendlyName);
            }
        }
        private void UpdateVolumeControl()
        {
            Debug.WriteLine("=======================UpdateVolumeControl======================");
            Debug.WriteLine("未完成");
            Debug.WriteLine("=======================UpdateVolumeControl======================");

        }

        //从Json文件中读取配置
        private void LoadConfig()
        {
            if(appConfig.ReadDeviceConfig())
            {
                    foreach (var deviceConfig in appConfig.GetDevicesConfig())
                {
                    Debug.WriteLine(deviceConfig.FriendlyName);
                    //如果该设备仍在使用，则添加该设备的信息
                    if (deviceInfoDict.ContainsKey(deviceConfig.FriendlyName)&& screenNameToScreenIndex.ContainsKey(deviceConfig.MonitorName))
                    {
                        Debug.WriteLine("Read " + deviceConfig.FriendlyName + " mesg");
                        //通过屏幕设备名称获取屏幕序号
                        int monitorIndex = screenNameToScreenIndex[deviceConfig.MonitorName];
                        screenIndexToAudioDeviceName[monitorIndex] = deviceConfig.FriendlyName;
                        deviceSelectControl.updateList(deviceConfig.MonitorName, deviceConfig.FriendlyName);
                        deviceNameToScreen[deviceConfig.FriendlyName] = screens[monitorIndex];
                        isBounded = true;
                    }
                }
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            AddUserControl();
            LoadConfig();
            //添加进程计数器,淘汰长时间未使用的进程
            Thread timerThread = new Thread(LookTimer);
            timerThread.Start();
            watcher = new ForegroundProcessWatcher();
            // 订阅事件
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

        //不能使用Close()
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
               
                appConfig.AddDeviceConfig(deviceName, screens[screenIndex].DeviceName,(int)deviceVolume);
                isBounded = true;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);


        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]

        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        // 定义枚举窗口的委托
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);



        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);



        public static string GetWindowTitle(IntPtr hWnd)
        {
            // 获取窗口标题的长度
            int length = GetWindowTextLength(hWnd);
            if (length == 0)
                return string.Empty;

            // 申请内存缓冲区来存储窗口标题
            StringBuilder stringBuilder = new StringBuilder(length + 1);

            // 获取窗口标题
            GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);

            return stringBuilder.ToString();
        }

        private void WinEventProcChangeAudioDevic(IntPtr hWinEventHook, uint eventType,
    IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            
            GetWindowThreadProcessId(hWnd, out uint processId);
            Debug.WriteLine("WinEventProc:Pid " + processId+" is foreground window");
            if (processInfoDict.ContainsKey(processId))
            {
                processInfoDict[processId].process = processInfoDict[processId].process;
                processInfoDict[processId].process.Refresh();
            }
            else
            {
                Debug.WriteLine("WinEventProc:not found process");
                return;
            }
            //Process process = Process.GetProcessById((int)processId);
            IntPtr mainWindowHandle = processInfoDict[processId].process.MainWindowHandle;

            if (mainWindowHandle != hWnd || mainWindowHandle == IntPtr.Zero)
            {
                Debug.WriteLine("WinEventProc:mainWindowHandle != hWnd");
                return;
            }
            // 获取窗口所在的屏幕

            IntPtr hMonitor = MonitorFromWindow(mainWindowHandle, 0x00000002);
            if (hMonitor == IntPtr.Zero)
            {
                Debug.WriteLine("WinEventProc:hMonitor == IntPtr.Zero");
                return;
            }
            // 获取屏幕信息

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
                //textBox1.Text += screenIndexToAudioDevice[processInfo.MonitorIndex] + processId;
                try
                {
                    audioSwitcher.SwitchProcessTo(device.ID, eRole, eDataFlow, processId);

                    Debug.WriteLine("WinEventProc:Switching audio process " + processId + " title is " + winTitle + " on Screen: " + processInfoDict[processId].MonitorIndex);
                }
                catch (Exception ex)
                {
                    // 处理异常，比如记录日志或者显示错误消息
                    MessageBox.Show("WinEventProc:An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            // 输出屏幕编号
            //textBox1.Text += "Process " + processId + " title is " + winTitle + " on Screen: " + processInfo.MonitorIndex + "\r\n";
        }
        // 导入Windows API函数

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);



        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }
        //监听进程窗口是否移动
        void CreateWinHook(int processId)
        {
            try
            {

                
                IntPtr hook = SetWinEventHook(0x000B, 0x000B, IntPtr.Zero, WinEventProcChangeAudioDevic, (uint)processId, 0, 0);
                processIdHookDict[processId] = hook;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //textBox1.Text += $"创建{GetWindowTitle(Process.GetProcessById(processid).MainWindowHandle)}{processid}'s hook\r\n";
        }

        void UnWinHook(IntPtr hook)
        {
            if (hook != IntPtr.Zero)
            {
                Invoke(new Action(() =>
                {
                    if (UnhookWinEvent(hook))
                    {   //只能在创建h ook的进程中卸载hook
                        Debug.WriteLine("Unhook success！");
                    }
                    else
                    {
                        Debug.WriteLine("unhook on thread" + Thread.CurrentThread.ManagedThreadId);
                        Debug.WriteLine("Unhook failed！");
                    }
                }
                ));
            }
        }

        private void Watcher_ForegroundProcessChanged(int processId)
        {
            /*
            //判断进程是否使用音频设备
            if (!IsUsingAudioDeviceByProcessId(processId))
            {
                //如果此时进程没有使用音频设备，判断是否已经添加到processInfoDict并卸载钩子
                if(processIdHookDict.ContainsKey(processId))
                {
                    UnWinHook(processIdHookDict[processId]);
                }
                return;
            }*/
            //判断进程是否已经添加到processInfoDict
            if (processInfoDict.ContainsKey((uint)processId))
            {
                processInfoDict[(uint)processId].resetTime();
                return;
            }
            //当进程被创建时，检测进程是否在
            ProcessInfo processInfo = new ProcessInfo();
            processInfo.process = Process.GetProcessById(processId);
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
