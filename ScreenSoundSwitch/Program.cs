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
            // ����һ�������Ļ����壬����Ѵ���ͬ���Ļ����壬�򲻴����µ�ʵ��
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
                    // ����������Ѵ��ڣ���˵���Ѿ���һ��ʵ�������У��˳���ǰʵ��
                    MessageBox.Show("Another instance of the application is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}