using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ScreenSoundSwitch.WinUI.Audio;
using System.Diagnostics;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Models;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProcessPage : Page
    {
        AudioDeviceManger audioDeviceManger;
        MMDeviceCollection mMDevices;
        ProcessControl foregroundProcess;
        public ProcessModel processModel => ((App)Application.Current).processModel;
        public ProcessPage()
        {
            this.InitializeComponent();
            audioDeviceManger = new AudioDeviceManger();
        }
        public void UpdateProcessBySeesion()
        {
            if (mMDevices == null)
            {
                mMDevices = audioDeviceManger.GetDevices();
            }
            ProcessStackPanel.Children.Clear();
            foreach (var device in mMDevices)
            {               
                var textDeviceName=new TextBlock();
                textDeviceName.Text = device.FriendlyName;
                ProcessStackPanel.Children.Add(textDeviceName);
                
                var sessions = device.AudioSessionManager.Sessions;
                for (int i=0;i< sessions.Count; i++)
                {
                    if (sessions[i].IsSystemSoundsSession)
                    {
                        continue;
                    }
                    ProcessStackPanel.Children.Add(new ProcessControl(sessions[i]));
                }
              
            }
        }

        public void UpdataForegroundProcess(uint processId)
        {
            foreach( var processControl in ProcessStackPanel.Children.OfType<ProcessControl>())
            {
                if (processId == processControl.ProcessId)
                {
                    foregroundProcess=processControl;
                    Debug.WriteLine(processControl.ProcessId);
                }
            }
        }
        public void UpdataForegroundVolume(int delta)
        {
            if(foregroundProcess == null)
            {
                return;
            }
            if (delta > 0)
            {
                foregroundProcess.ChangeSimpleVolumeLevel(0.05f);
            }
            else
            {
                foregroundProcess.ChangeSimpleVolumeLevel(-0.05f);
            }
        }
        /// <summary>
        /// �����������Ӧ�Ĵ��ڵ��ƶ��¼����жϽ����Ƿ�ӵ�ǰ��������ʾ�����ƶ�����һ����ʾ���ϣ����������processModel���ṩ���豸������л��ý��̵Ĳ����豸
        /// </summary>
        /// <param name="hwnd">��ǰ���ڱ��ƶ��ı��۽��Ĵ��ڵľ��</param>
        /// <param name="pid">��Ӧ���ڵĽ���id</param>
        public void ForegroundMovedHandle(IntPtr hwnd, uint pid)
        {
            throw new NotImplementedException();
        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            mMDevices = audioDeviceManger.GetDevices();
            audioDeviceManger.SetProcessVolume(mMDevices, 27408, 0.1f);
            Debug.WriteLine($"set volume");
        }
    }
}
