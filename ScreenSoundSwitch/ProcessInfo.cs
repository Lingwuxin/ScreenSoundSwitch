using System.Diagnostics;
using System.Runtime.InteropServices;


namespace ScreenSoundSwitch
{
    class ProcessInfo
    {
        Screen[] allScreens = Screen.AllScreens;

        public Process process { get; set; }
        public int MonitorIndex {
            get { return GetMonitorIndex(); }
            set { MonitorIndex = value; }
        }

        public int lastMonitorIndex { get; set; } = -1;


        private int GetMonitorIndex()
        {
            
            int monitorIndex = -1; // 默认为-1表示找不到
            Screen targetScreen=Screen.FromHandle(process.MainWindowHandle);
            
            for(int i = 0; i < allScreens.Length; i++)
            {
                if (allScreens[i].Equals(targetScreen))
                {
                    monitorIndex = i;
                    break;
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
        private CancellationTokenSource cts = new CancellationTokenSource();
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

                while (!cts.Token.IsCancellationRequested)
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
