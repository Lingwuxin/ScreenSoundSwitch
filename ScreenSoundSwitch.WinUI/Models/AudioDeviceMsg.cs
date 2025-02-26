using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Models
{

    class AudioDeviceMsg
    {
        public string DeviceName;
        AudioDeviceMsg(string deviceName)
        {
            DeviceName = deviceName;
        }

    }
}
