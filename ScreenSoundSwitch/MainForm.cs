//v0.0.1
using SoundSwitch.Audio.Manager;
using SoundSwitch.Audio.Manager.Interop.Enum;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
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
        ForegroundProcessWatcher watcher;
        private NotifyIcon notifyIcon;
        public MainForm()
        {
            InitializeComponent();
            InitializeNotifyIcon();
        }
        // ���� MonitorFromWindow ����
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        // ���� GetMonitorInfo ����
        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        // ���� MONITORINFOEX �ṹ��
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
        // ���� SetWinEventHook ����
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]

        // ���� UnhookWinEvent ����
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        // ���� WinEvent ί������
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        // ���� WinEvent ����
        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
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


        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
    IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Debug.WriteLine("WinEventProc");
            // ����¼������Ƿ�Ϊ����λ�øı�
            if (eventType != 0x000B)
            {
                return;
            }
            // ɸѡ�ɼ�����
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

            if (mainWindowHandle != hWnd||mainWindowHandle==IntPtr.Zero)
            {
                return;
            }
            // ��ȡ�������ڵ���Ļ

            IntPtr hMonitor = MonitorFromWindow(mainWindowHandle, 0x00000002);
            if (hMonitor == IntPtr.Zero)
            {
                return;
            }
            // ��ȡ��Ļ��Ϣ
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
                Debug.WriteLine("No audio device selected for screen " + processInfoDict[processId].MonitorIndex);
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
            }catch(Exception ex)
            {
                MessageBox.Show("An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //textBox1.Text += $"����{GetWindowTitle(Process.GetProcessById(processid).MainWindowHandle)}{processid}'s hook\r\n";
        }
        void Watcher_HookNeedDisable(IntPtr hook)
        {
            if(hook != IntPtr.Zero)
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
            processInfoDict[(uint)processId]=processInfo;
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
                        watcher.DisableHook(processIdHookDict[processid]);
                        processIdHookDict.Remove(processid);
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
            // ��ȡ������ʾ�������Ƕ�Ӧ�Ĳ����豸
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
            //��ӽ��̼�����,��̭��ʱ��δʹ�õĽ���
            Thread timerThread = new Thread(LookTimer);
            timerThread.Start();
            watcher = new ForegroundProcessWatcher();
            // �����¼�
            watcher.ForegroundProcessChanged += Watcher_ForegroundProcessChanged;
            watcher.HookNeedDisable += Watcher_HookNeedDisable;
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

            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("�˳�");
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);

            notifyIcon.ContextMenuStrip = contextMenu;
        }
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
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
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {   
            notifyIcon.Dispose();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (comboBoxAudio.SelectedItem != null)
            {
                screenIndexToAudioDevice[comboBoxScreen.SelectedIndex] = (string)comboBoxAudio.SelectedItem;
            }

        }
    }
}
