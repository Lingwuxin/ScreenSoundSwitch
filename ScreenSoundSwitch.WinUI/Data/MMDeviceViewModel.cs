using NAudio.CoreAudioApi;
using System.ComponentModel;

namespace ScreenSoundSwitch.WinUI.Models
{
    public class MMDeviceViewModel : INotifyPropertyChanged
    {
        private MMDeviceCollection _mMDevices;
        public MMDeviceCollection ShareDate
        {
            get => _mMDevices;
            set
            {
                _mMDevices = value;
                OnPropertyChanged(nameof(ShareDate));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
