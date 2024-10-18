using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    public sealed partial class AudioDeviceControl : UserControl
    {
        MMDevice device;
        bool block=false;
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
            block= false;

        }
    }
}
