using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenSoundSwitch.WinUI.Models
{
    /// <summary>
    /// 在SelectDevicePage中向ProcessPage传递设备组合信息
    /// </summary>
    public class ProcessModel: INotifyPropertyChanged
    {
        private Dictionary<Screen, MMDevice> _sharedData;
        public Dictionary<Screen, MMDevice> SharedData
        {
            get => _sharedData;
            set
            {
                _sharedData = value;
                OnPropertyChanged(nameof(SharedData));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
