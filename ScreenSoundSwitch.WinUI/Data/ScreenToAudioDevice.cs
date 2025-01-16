using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenSoundSwitch.WinUI.Data
{
    class ScreenToAudioDevice:Dictionary<Screen,MMDevice>
    {
        private static ScreenToAudioDevice _Instance;
        private ScreenToAudioDevice()
        {
            
        }
        public static ScreenToAudioDevice Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ScreenToAudioDevice();
                }
                return _Instance;
            }
        }
    }
}
