using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using NAudio.CoreAudioApi;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    public sealed partial class AudioDeviceControl : UserControl
    {
        MMDevice device;
        bool block = false;
        private AudioEndpointVolume audioEndpointVolume;
        public AudioDeviceControl()
        {
            this.InitializeComponent();
        }
        public AudioDeviceControl(MMDevice device)
        {
            this.InitializeComponent();
            this.device = device;
            UpdateDeviceMsg();
        }

        private void RightChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (block)
                {
                    return;
                }
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {

                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            device.AudioEndpointVolume.Channels[1].VolumeLevelScalar = (float)(e.NewValue / 100);
                        }
                    }
                }
                catch
                {
                    throw new NotImplementedException("Error: RightChannelSlider_ValueChanged");
                }
            });

        }

        private void LeftChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (block)
                {
                    return;
                }
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {
                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            device.AudioEndpointVolume.Channels[0].VolumeLevelScalar = (float)(e.NewValue / 100);
                        }
                    }

                }
                catch
                {
                    throw new NotImplementedException("Error: LeftChannelSlider_ValueChanged");
                }
            });


        }

        private void MainVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (block)
                {
                    return;
                }
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
            DeviceNameTextBlock.Text = device.FriendlyName;
            MainVolumeSlider.Value = device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
            if (device.AudioEndpointVolume.Channels.Count == 2)
            {
                LeftChannelSlider.Value = device.AudioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                RightChannelSlider.Value = device.AudioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
            }
            audioEndpointVolume.OnVolumeNotification += MasterVolumeChanged;

        }

        public void MasterVolumeChanged(AudioVolumeNotificationData data)
        {
            block = true;
            DispatcherQueue.TryEnqueue(() =>
            {
                // 安全地更新 UI 组件

                MainVolumeSlider.Value = audioEndpointVolume.MasterVolumeLevelScalar * 100;

                if (data.Channels == 2)
                {
                    LeftChannelSlider.Value = audioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                    RightChannelSlider.Value = audioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
                }
            });
            block = false;

        }
    }
}
