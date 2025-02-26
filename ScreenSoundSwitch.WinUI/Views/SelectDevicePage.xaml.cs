using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Audio;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.Models;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Application = Microsoft.UI.Xaml.Application;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectDevicePage : Page
    {
        private ScreenToAudioDevice screenToAudioDevice;
        private AudioDeviceManager audioDeviceManager;
        public SelectDevicePage()
        {
            
            this.InitializeComponent();
            screenToAudioDevice = ScreenToAudioDevice.Instance;
            audioDeviceManager = AudioDeviceManager.Instance;
            //UpdateScreenSelection();
            //UpdateDeviceSelection();
        }


    }

}
