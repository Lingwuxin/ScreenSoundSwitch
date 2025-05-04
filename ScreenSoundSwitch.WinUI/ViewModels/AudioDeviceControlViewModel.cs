using CommunityToolkit.Mvvm.ComponentModel;
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
