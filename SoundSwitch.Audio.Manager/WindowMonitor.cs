using Serilog;
using SoundSwitch.Audio.Manager.Interop.Com.Threading;
using SoundSwitch.Audio.Manager.Interop.Com.User;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static SoundSwitch.Audio.Manager.Interop.Com.User.User32.NativeMethods;
namespace SoundSwitch.Audio.Manager
{
    public class WindowMonitor
    {
        public class Event : EventArgs
        {
            /// <summary>
            /// ID of the process that is now active
            /// </summary>
            public uint ProcessId { get; }

            /// <summary>
            /// Name of the process that is active
            /// </summary>
            public string ProcessName { get; }

            /// <summary>
            /// Name of the active window
            /// </summary>
            public string WindowName { get; }

            /// <summary>
            /// Class of the window
            /// </summary>
            public string WindowClass { get; }

            /// <summary>
            /// Handle of the window
            /// </summary>
            public User32.NativeMethods.HWND Hwnd { get; }

            public Event(uint processId, string processName, string windowName, string windowClass, User32.NativeMethods.HWND hwnd)
            {
                ProcessId = processId;
                ProcessName = processName;
                WindowName = windowName;
                WindowClass = windowClass;
                Hwnd = hwnd;
            }

            public override string ToString()
            {
                return $"{nameof(ProcessId)}: {ProcessId}, {nameof(ProcessName)}: {ProcessName}, {nameof(WindowName)}: {WindowName}, {nameof(WindowClass)}: {WindowClass}";
            }
        }
        public class MouseWheelEventArgs : EventArgs
        {
            public int Delta { get; }
            public MouseWheelEventArgs(int delta)
            {
                Delta = delta;
            }
        }



        public event EventHandler<Event> ForegroundChanged;
        public event EventHandler<Event> ForegroundWindowMoved;
        public event EventHandler<MouseWheelEventArgs> MouseWheelScrolled;
        private readonly User32.NativeMethods.WinEventDelegate _foregroundWindowChanged;
        private readonly User32.NativeMethods.WinEventDelegate _foregroundWindowMoved;
        private readonly User32.NativeMethods.HookProc _mouseProc;
        private IntPtr _keyboardHookID = IntPtr.Zero;
        private IntPtr _mouseHookID = IntPtr.Zero;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public WindowMonitor()
        {
            _foregroundWindowChanged = (hook, type, hwnd, idObject, child, thread, time) =>
            {
                // ignore any event not pertaining directly to the window
                if (idObject != User32.NativeMethods.OBJID_WINDOW)
                    return;

                // Ignore if this is a bogus hwnd (shouldn't happen)
                if (hwnd == IntPtr.Zero)
                    return;
                var (processId, windowText, windowClass) = ProcessWindowInformation(hwnd);

                //Couldn't find the processId of the window
                if (processId == 0) return;

                if (processId == Environment.ProcessId)
                {
                    Log.Information("Foreground = SoundSwitch, don't save.");
                    return;
                }

                Task.Factory.StartNew(() =>
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        return;
                    try
                    {
                        var process = Process.GetProcessById((int)processId);
                        var processName = process.MainModule?.FileName ?? "N/A";
                        ForegroundChanged?.Invoke(this, new Event(processId, processName, windowText, windowClass, hwnd));
                    }
                    catch (Exception)
                    {
                        //Ignored 
                    }
                }, _cancellationTokenSource.Token);
            };
            _foregroundWindowMoved = (hook, type, hwnd, idObject, child, thread, time) =>
            {
                // Ignore any event not pertaining directly to the window
                if (idObject != User32.NativeMethods.OBJID_WINDOW)
                    return;

                // Ignore if this is a bogus hwnd (shouldn't happen)
                if (hwnd == IntPtr.Zero)
                    return;

                var (processId, windowText, windowClass) = ProcessWindowInformation(hwnd);

                // Couldn't find the processId of the window
                if (processId == 0) return;

                if (processId == Environment.ProcessId)
                {
                    Log.Information("Window moved = SoundSwitch, don't save.");
                    return;
                }

                Task.Factory.StartNew(() =>
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        return;

                    try
                    {
                        var process = Process.GetProcessById((int)processId);
                        var processName = process.MainModule?.FileName ?? "N/A";
                        ForegroundWindowMoved?.Invoke(this, new Event(processId, processName, windowText, windowClass, hwnd));
                    }
                    catch (Exception)
                    {
                        // Ignored
                    }
                }, _cancellationTokenSource.Token);
            };


            ComThread.Invoke(() =>
            {
                User32.NativeMethods.SetWinEventHook(User32.NativeMethods.EVENT_SYSTEM_MOVESIZEEND,
                    User32.NativeMethods.EVENT_SYSTEM_MOVESIZEEND,
                    IntPtr.Zero, _foregroundWindowMoved,
                    0,
                    0,
                    User32.NativeMethods.WINEVENT_OUTOFCONTEXT);
            });

            ComThread.Invoke(() =>
            {
                User32.NativeMethods.SetWinEventHook(User32.NativeMethods.EVENT_SYSTEM_MINIMIZEEND,
                    User32.NativeMethods.EVENT_SYSTEM_MINIMIZEEND,
                    IntPtr.Zero, _foregroundWindowChanged,
                    0,
                    0,
                    User32.NativeMethods.WINEVENT_OUTOFCONTEXT);

                User32.NativeMethods.SetWinEventHook(User32.NativeMethods.EVENT_SYSTEM_FOREGROUND,
                    User32.NativeMethods.EVENT_SYSTEM_FOREGROUND,
                    IntPtr.Zero, _foregroundWindowChanged,
                    0,
                    0,
                    User32.NativeMethods.WINEVENT_OUTOFCONTEXT);
            });
            _mouseProc = HookCallbackMouse;
            ComThread.Invoke(() =>
            {
                _mouseHookID = SetHook(_mouseProc, WH_MOUSE_LL);
            });
        }

        private IntPtr SetHook(HookProc proc, int evenType)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(evenType, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEWHEEL)
            {
                MSLLHOOKSTRUCT mouseHookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                int delta = (short)(mouseHookStruct.mouseData >> 16);
                // 如果 Ctrl 和 Alt 被按下，触发事件
                if ((GetKeyState(VK_CONTROL) & 0x8000) != 0 && (GetKeyState(VK_MENU) & 0x8000) != 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                            return;

                        try
                        {
                            MouseWheelScrolled?.Invoke(this, new MouseWheelEventArgs(delta));
                        }
                        catch (Exception)
                        {
                            // Ignored
                        }
                    }, _cancellationTokenSource.Token);

                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }
        private static bool IsKeyPressed(int key)
        {
            return (User32.NativeMethods.GetAsyncKeyState(key) & 0x8000) != 0;
        }
        public static (uint ProcessId, string WindowText, string WindowClass) ProcessWindowInformation(User32.NativeMethods.HWND hwnd)
        {
            return ComThread.Invoke(() =>
            {
                uint processId = 0;
                var wndText = "";
                var wndClass = "";
                try
                {
                    wndText = User32.GetWindowText(hwnd);
                }
                catch (Exception)
                {
                    // ignored
                }

                try
                {
                    wndClass = User32.GetWindowClass(hwnd);
                }
                catch (Exception)
                {
                    // ignored
                }

                try
                {
                    User32.NativeMethods.GetWindowThreadProcessId(hwnd, out processId);
                }
                catch (Exception)
                {
                    // ignored
                }


                return (processId, wndText, wndClass);
            });

        }
        public void Stop()
        {
            // 请求取消所有正在运行的任务
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public void Dispose()
        {
            Stop(); // 调用 Stop 方法清理资源
        }
    }

}