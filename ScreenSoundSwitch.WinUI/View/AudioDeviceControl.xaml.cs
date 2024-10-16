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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    public sealed partial class AudioDeviceControl : UserControl
    {
        MMDevice device;
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
        public void UpdateDeviceMsg()
        {
            DeviceNameTextBlock.Text = device.FriendlyName;
            MainVolumeSlider.Value = device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
         
        }
    }
}
