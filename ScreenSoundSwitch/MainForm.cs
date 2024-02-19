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
            textBox1.Text += "������\r\n";
            // ����¼������Ƿ�Ϊ����λ�øı�
            if (eventType == EVENT_OBJECT_LOCATIONCHANGE)
            {
                string winTitle = GetWindowTitle(hWnd);
                // ɸѡ�ɼ�����
                if (IsWindowVisible(hWnd) && winTitle.Length > 0)
                {
                    // ��ȡ������������ID
                    GetWindowThreadProcessId(hWnd, out uint processId);

                    // ��ȡ���̶���
                    Process process = Process.GetProcessById((int)processId);

                    // ��ȡ�����ھ��
                    IntPtr mainWindowHandle = process.MainWindowHandle;

                    if (mainWindowHandle != IntPtr.Zero && mainWindowHandle == hWnd)
                    {
                        // ��ȡ�������ڵ���Ļ

                        IntPtr hMonitor = MonitorFromWindow(mainWindowHandle, 0x00000001);
                        if (hMonitor != IntPtr.Zero)
                        {
                            // ��ȡ��Ļ��Ϣ
                            MONITORINFOEX monitorInfo = new MONITORINFOEX();
                            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                            GetMonitorInfo(hMonitor, ref monitorInfo);

                            // ���� ProcessInfo ���������Ϣ
                            ProcessInfo processInfo = new ProcessInfo();
                            processInfo.MainWindowHandle = mainWindowHandle;
                            processInfo.ProcessId = processId;
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

                            processInfo.MonitorIndex = closestMonitorIndex;

                            // �� ProcessInfo �������ӵ��ֵ���
                            if (processInfoDict.ContainsKey(processId) && processInfoDict[processId].MonitorIndex != processInfo.MonitorIndex)
                            {
                                textBox1.Text += "Process " + processId + " title is " + winTitle + " on Screen: " + processInfo.MonitorIndex + "\r\n";
                                if (screenIndexToAudioDevice.ContainsKey(processInfo.MonitorIndex))
                                {
                                    MMDevice? device = deviceInfoDict[screenIndexToAudioDevice[processInfo.MonitorIndex]];
                                    if (device != null)
                                    {
                                        EDataFlow eDataFlow = new EDataFlow();
                                        ERole eRole = new ERole();
                                        //textBox1.Text += screenIndexToAudioDevice[processInfo.MonitorIndex] + processId;
                                        try
                                        {
                                            audioSwitcher.SwitchProcessTo(device.ID, eRole, eDataFlow, processId);
                                        }
                                        catch (Exception ex)
                                        {
                                            // �����쳣�������¼��־������ʾ������Ϣ
                                            MessageBox.Show("An error occurred while switching audio process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        }
                                    }
                                }
                            }
                            processInfoDict[processId] = processInfo;

                            // �����Ļ���
                            textBox1.Text += "Process " + processId + " title is " + winTitle + " on Screen: " + processInfo.MonitorIndex + "\r\n";
                        }
                        else
                        {
                            Console.WriteLine("Failed to get monitor information.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Process does not have a main window.");
                    }
                }
            }
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
            IntPtr hook = SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE,
          IntPtr.Zero, WinEventProc, (uint)processid, 0, 0);
            processIdHookDict[processid]=hook;
            textBox1.Text += $"����{GetWindowTitle(Process.GetProcessById(processid).MainWindowHandle)}{processid}'s hook\r\n";
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
            
            string wintitle = Process.GetProcessById(processId).MainWindowTitle;
            
            // ��ӡǰ̨���̵� ID
            textBox1.Text += wintitle+"\r\n";
            if(processIdHookDict.ContainsKey(processId)==false)
            {
                //ȱ�ٶ���ж�ع��ӵĹ���
                CreateWinHook(processId);
    
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // ��ȡ������ʾ�������Ƕ�Ӧ�Ĳ����豸
            AudioDeviceEnumerator audioDeviceEnumerator = new AudioDeviceEnumerator();
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
            ForegroundProcessWatcher watcher = new ForegroundProcessWatcher();

            // �����¼�
            watcher.ForegroundProcessChanged += Watcher_ForegroundProcessChanged;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("ȷ��Ҫ�رմ�����", "ȷ�Ϲر�", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; // ȡ���رղ���
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