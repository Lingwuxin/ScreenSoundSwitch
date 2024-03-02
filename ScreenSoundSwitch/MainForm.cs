//v0.0.1
using SoundSwitch.Audio.Manager;
using SoundSwitch.Audio.Manager.Interop.Enum;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.CoreAudioApi.Interfaces;
namespace ScreenSoundSwitch
{
    public partial class MainForm : Form
    {
        AudioSwitcher audioSwitcher = AudioSwitcher.Instance;
        MMDeviceCollection deviceCollection;
        Screen[] screens;
        Dictionary<uint, ProcessInfo> processInfoDict = new Dictionary<uint, ProcessInfo>();
        Dictionary<string, MMDevice?> deviceInfoDict = new Dictionary<string, MMDevice?>();
        Dictionary<int, string> screenIndexToAudioDevice = new Dictionary<int, string>();
        Dictionary<int, IntPtr> processIdHookDict = new Dictionary<int, IntPtr>();
        public MainForm()
        {
            InitializeComponent();

        }

        // 导入 MonitorFromWindow 函数
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        // 导入 GetMonitorInfo 函数
        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        // 定义 MONITORINFOEX 结构体
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
        // 导入 SetWinEventHook 函数
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]

        // 导入 UnhookWinEvent 函数
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        // 定义 WinEvent 委托类型
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        // 定义 WinEvent 常量
        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        // 定义枚举窗口的委托
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


        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
    IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // 检查事件类型是否为窗口位置改变
            if (eventType != 0x000B)
            {
                return;
               
            }

            // 筛选可见窗口

            // 获取窗口所属进程ID
            GetWindowThreadProcessId(hWnd, out uint processId);
 
            
            if (processInfoDict.ContainsKey(processId))
            {
                processInfoDict[processId].process = processInfoDict[processId].process;
                processInfoDict[processId].process.Refresh();
            }
            else
            {
                textBox1.Text += "not found process\r\n";
                return;
            }
            //Process process = Process.GetProcessById((int)processId);
            IntPtr mainWindowHandle = processInfoDict[processId].process.MainWindowHandle;

            if (mainWindowHandle != hWnd||mainWindowHandle==IntPtr.Zero)
            {
                return;
            }
            // 获取窗口所在的屏幕

            IntPtr hMonitor = MonitorFromWindow(mainWindowHandle, 0x00000002);
            if (hMonitor == IntPtr.Zero)
            {
                return;
            }
            // 获取屏幕信息
            MONITORINFOEX monitorInfo = new MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            GetMonitorInfo(hMonitor, ref monitorInfo);

            RECT windowRect = new RECT();
            GetWindowRect(mainWindowHandle, ref windowRect);

            int closestMonitorIndex = -1;
            int closestDistance = int.MaxValue;

            Screen[] allScreens = Screen.AllScreens;
            for (int i = 0; i < allScreens.Length; i++)
            {
                Rectangle monitorBounds = allScreens[i].Bounds;
                int distance = Math.Abs(windowRect.Left - monitorBounds.Left) +
                                Math.Abs(windowRect.Top - monitorBounds.Top);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMonitorIndex = i;
                }
            }
            if(processInfoDict[processId].MonitorIndex.Equals(closestMonitorIndex))
            {
                return;
            }
            string winTitle = GetWindowTitle(mainWindowHandle);
            processInfoDict[processId].MonitorIndex = closestMonitorIndex;

            
            if (!screenIndexToAudioDevice.ContainsKey(processInfoDict[processId].MonitorIndex))
            {
                return;
            }
            string screenIndex = screenIndexToAudioDevice[processInfoDict[processId].MonitorIndex];
            if (deviceInfoDict[screenIndex] == null)
            {
                textBox1.Text += "No audio device selected for screen " + processInfoDict[processId].MonitorIndex + "\r\n";
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

                    textBox1.Text += "Switching audio process " + processId + " title is " + winTitle + " on Screen: " + processInfoDict[processId].MonitorIndex+"\r\n";
                }
                catch (Exception ex)
                {
                    // 处理异常，比如记录日志或者显示错误消息
                    MessageBox.Show("An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            // 输出屏幕编号
            //textBox1.Text += "Process " + processId + " title is " + winTitle + " on Screen: " + processInfo.MonitorIndex + "\r\n";
        }


        // 导入Windows API函数
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


        //监听进程窗口是否移动
        void CreateWinHook(int processid)
        {
            try
            {
                IntPtr hook = SetWinEventHook(0x000B, 0x000B, IntPtr.Zero, WinEventProc, (uint)processid, 0, 0);
                processIdHookDict[processid] = hook;
            }catch(Exception ex)
            {
                MessageBox.Show("An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //textBox1.Text += $"创建{GetWindowTitle(Process.GetProcessById(processid).MainWindowHandle)}{processid}'s hook\r\n";
        }
        bool UnWinHook(IntPtr hook)
        {
            if(hook != IntPtr.Zero)
            {
                UnhookWinEvent(hook);
                return true;
            }
            return false;
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
            ProcessInfo processInfo = new ProcessInfo();
            processInfo.process = Process.GetProcessById(processId);
            processInfoDict[(uint)processId]=processInfo;
            //缺少定期卸载钩子的功能(已添加，待测试）
            this.Invoke(new Action(() =>
            {
                CreateWinHook(processId);
            }));
            textBox1.Text+="Create hook for process "+processId+"\r\n";

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
                        UnhookWinEvent(processIdHookDict[processid]);
                        processIdHookDict.Remove(processid);
                        textBox1.Text += "process " + processid + " is timeout," + processIdHookDict.Count + "\r\n";
                    }
                    else
                    {
                        processInfoDict[key].countTime();
                    }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 获取所有显示器和它们对应的播放设备
            AudioDeviceManger audioDeviceEnumerator = new AudioDeviceManger();
            deviceCollection = audioDeviceEnumerator.GetDevices();
            foreach (var device in deviceCollection)
            {
                comboBoxAudio.Items.Add(device.FriendlyName);
                deviceInfoDict.Add(device.FriendlyName, device);

            }
            screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                comboBoxScreen.Items.Add(i + "." + screens[i].DeviceName);
            }
            //添加进程计数器,淘汰长时间未使用的进程
            Thread timerThread = new Thread(LookTimer);
            timerThread.Start();
            ForegroundProcessWatcher watcher = new ForegroundProcessWatcher();
            // 订阅事件
            watcher.ForegroundProcessChanged += Watcher_ForegroundProcessChanged;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要关闭窗口吗？", "确认关闭", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; // 取消关闭操作
            }
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {   
            foreach (var hook in processIdHookDict)
            {
                UnWinHook(hook.Value);
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (comboBoxAudio.SelectedItem != null)
            {
                screenIndexToAudioDevice[comboBoxScreen.SelectedIndex] = (string)comboBoxAudio.SelectedItem;
                textBox1.Text += comboBoxScreen.SelectedIndex + "is on" + (string)comboBoxAudio.SelectedItem + "\r\n";
            }

        }
    }
}
