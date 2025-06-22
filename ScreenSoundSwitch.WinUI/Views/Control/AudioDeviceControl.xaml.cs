using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    public sealed partial class AudioDeviceControl : UserControl
    {
        private AudioDeviceControlViewModel viewModel;
        MMDevice device;
        private AudioEndpointVolume audioEndpointVolume;
        public StackPanel _ProcessStackPanel;
        
        public AudioDeviceControl()
        {
            this.InitializeComponent();
            viewModel=this.DataContext as AudioDeviceControlViewModel;
        }
        public AudioDeviceControl(MMDevice device)
        {
            this.InitializeComponent();
            viewModel = this.DataContext as AudioDeviceControlViewModel;
            this.device = device;
            this._ProcessStackPanel = ProcessStackPanel;
            UpdateDeviceMsg();
            UpdateProcessSession();
        }

        private void SessionList_Expanded(Expander sender, ExpanderExpandingEventArgs args)
        {
            UpdateProcessSession();
        }
        public void UpdateProcessSession()
        {
            ProcessStackPanel.Children.Clear();
            device.AudioSessionManager.RefreshSessions();
            var sessions = device.AudioSessionManager.Sessions;
            for (int i = 0; i < sessions.Count; i++)
            {
                if (sessions[i].IsSystemSoundsSession)
                {
                    continue;
                }
                ProcessStackPanel.Children.Add(new ProcessControl(sessions[i]));
            }
        }

        private void RightChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {

                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            device.AudioEndpointVolume.Channels[1].VolumeLevelScalar = (float)(e.NewValue / 100);
                            RightChannelVolumeText.Text = ((int)e.NewValue).ToString();
                        }
                    }
                }
                catch
                {
                    throw new Exception("Error: RightChannelSlider_ValueChanged");
                }
            });

        }

        private void LeftChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {
                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            device.AudioEndpointVolume.Channels[0].VolumeLevelScalar = (float)(e.NewValue / 100);
                            LeftChannelVolumeText.Text = ((int)e.NewValue).ToString();
                        }
                    }

                }
                catch
                {
                    throw new Exception("Error: LeftChannelSlider_ValueChanged");
                }
            });


        }

        private void MainVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {
                        device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(e.NewValue / 100);
                    }
                }
                catch
                {
                    throw new NotImplementedException("Error: MainVolumeSlider_ValueChanged");
                }
            });


        }

        public void UpdateDeviceMsg()
        {
            audioEndpointVolume = device.AudioEndpointVolume;
            viewModel.SetDeviceName(device.FriendlyName) ;
            DispatcherQueue.TryEnqueue(() =>
            {
                MainVolumeSlider.Value = device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                if (device.AudioEndpointVolume.Channels.Count == 2)
                {
                    LeftChannelSlider.Value = device.AudioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                    RightChannelSlider.Value = device.AudioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
                }
            });
            audioEndpointVolume.OnVolumeNotification += MasterVolumeChanged;

        }

        public void MasterVolumeChanged(AudioVolumeNotificationData data)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                // 安全地更新 UI 组件
                if (data.EventContext != Guid.Empty)
                {
                    //由自身引起的音量变化不做响应
                    MainVolumeSlider.Value = audioEndpointVolume.MasterVolumeLevelScalar * 100;

                    if (data.Channels == 2)
                    {
                        LeftChannelSlider.Value = audioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                        RightChannelSlider.Value = audioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
                    }
                }
            });

        }
    }
}
