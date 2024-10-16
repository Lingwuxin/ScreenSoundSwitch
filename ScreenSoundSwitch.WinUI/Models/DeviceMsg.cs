using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Models
{
    public class DeviceMsg
    {
        public string DeviceName { get; set; }
        public DeviceMsg(string deviceName)
        {
            DeviceName = deviceName;
        }
    }
}
