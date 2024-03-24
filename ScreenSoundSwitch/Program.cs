using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace ScreenSoundSwitch
{
    internal static class Program
    {


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        ///
        private const string MutexName = "ScreenSoundSwitchMutex";

        [STAThread]
        static void Main()
        {
            bool createdNew;
            // 创建一个命名的互斥体，如果已存在同名的互斥体，则不创建新的实例
            using (Mutex mutex = new Mutex(true, MutexName, out createdNew))
            {
                if (createdNew)
                {
                    // To customize application configuration such as set high DPI settings or default font,
                    // see https://aka.ms/applicationconfiguration.
                    ApplicationConfiguration.Initialize();
                    Application.Run(new MainForm());
                }
                else
                {
                    // 如果互斥体已存在，则说明已经有一个实例在运行，退出当前实例
                    MessageBox.Show("Another instance of the application is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}