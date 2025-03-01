using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenSoundSwitch.WinUI.Models;

namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class AudioDeviceControlViewModel: ObservableObject
    {
        [ObservableProperty]
        public partial AudioDeviceControlModel AudioDeviceControlModel { get; set; }
        public AudioDeviceControlViewModel()
        {
            AudioDeviceControlModel=new AudioDeviceControlModel();
        }

        internal void SetDeviceName(string friendlyName)
        {
            AudioDeviceControlModel.DeviceName = friendlyName;
        }
    }
}
