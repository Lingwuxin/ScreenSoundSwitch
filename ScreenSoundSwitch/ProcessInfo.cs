using NAudio.CoreAudioApi.Interfaces;
using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Media.Core;
using Windows.Media.Streaming.Adaptive;
using System.Threading;

namespace ScreenSoundSwitch
{
    class ProcessInfo
    {
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
        private MONITORINFO monitorInfo;
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
        public Process process { get; set; }
        public int MonitorIndex {
            get { return GetMonitorIndex(); }  
            set { MonitorIndex = value; }
        }

        public int lastMonitorIndex { get; set; } = -1;


        private int GetMonitorIndex()
        {
            
            int monitorIndex = -1; // 默认为-1表示找不到
            IntPtr hWnd = process.MainWindowHandle;
            if (hWnd != IntPtr.Zero)
            {
                IntPtr hMonitor = MonitorFromWindow(hWnd, 0);
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                if (GetMonitorInfo(hMonitor, ref monitorInfo))
                {
                    monitorIndex = (int)monitorInfo.dwFlags;
                }
            }
            return monitorIndex;
        }
        private uint timeout = 225;
        private uint timer = 0;
        public bool IsUsing { get; set; }
        public void countTime()
        {
            timer++;
        }
        public void resetTime()
        {
            timer = 0;
        }
        public bool isTimeOut()
        {
            if (timer > timeout)
            {
                IsUsing = false;
                return true;
            }
            return false;
        }
    }
    public class ForegroundProcessWatcher
    {
        // 导入 Windows API 函数
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // 定义事件委托
        public delegate void ForegroundProcessChangedEventHandler(int processId);
        public delegate void HookNeedDisableEventHandler(IntPtr hook);

        // 定义事件
        public event ForegroundProcessChangedEventHandler ForegroundProcessChanged;
        public event HookNeedDisableEventHandler HookNeedDisable;
        private List<Thread>  threads = new List<Thread>();

        // 构造函数
        public ForegroundProcessWatcher()
        {
            // 启动监听
            StartWatching();
        }

        // 开始监听
        private void StartWatching()
        {
            // 启动后台线程进行监听
            Thread bgThread = new Thread(() =>
            {
                int lastProcessId = -1;

                while (true)
                {
                    IntPtr hWnd = GetForegroundWindow(); // 获取当前聚焦的窗口句柄
                    uint processId;
                    GetWindowThreadProcessId(hWnd, out processId); // 获取窗口所属进程的 ID

                    // 如果进程 ID 发生变化，则触发事件
                    if (lastProcessId != (int)processId)
                    {
                        lastProcessId = (int)processId;
                        OnForegroundProcessChanged(lastProcessId);
                    }
                    // 等待一段时间再次检查
                    Thread.Sleep(100);
                }
            });

            // 启动后台线程
            bgThread.IsBackground = true;
            bgThread.Start();
            threads.Add(bgThread);
        }
        
        // 禁用钩子
        public void DisableHook(IntPtr hook)
        {
            HookNeedDisable?.Invoke(hook);
        }
        // 触发事件的方法
        protected virtual void OnForegroundProcessChanged(int processId)
        {
            ForegroundProcessChanged?.Invoke(processId);
        }
    }


}
