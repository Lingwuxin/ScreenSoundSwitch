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
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProcessPage : Page
    {
        AudioDeviceManger audioDeviceManger=new AudioDeviceManger();
        
        public ProcessPage()
        {
            this.InitializeComponent();

        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            MMDeviceCollection mMDevices = audioDeviceManger.GetDevices();
            audioDeviceManger.SetProcessVolume(mMDevices, 27408, 0.1f);
            Debug.WriteLine($"set volume");
        }
    }
}
