using NAudio.CoreAudioApi.Interfaces;
using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ScreenSoundSwitch
{
    class ProcessInfo
    {
        public Process process { get; set; }
        public int MonitorIndex { get; set; }
        public bool IsUsing { get; set; }
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

        // 定义事件
        public event ForegroundProcessChangedEventHandler ForegroundProcessChanged;

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
            System.Threading.Thread bgThread = new System.Threading.Thread(() =>
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
                    System.Threading.Thread.Sleep(100);
                }
            });

            // 启动后台线程
            bgThread.IsBackground = true;
            bgThread.Start();
        }

        // 触发事件的方法
        protected virtual void OnForegroundProcessChanged(int processId)
        {
            ForegroundProcessChanged?.Invoke(processId);
        }
    }
}
