using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScreenSoundSwitch.WinUI.Data
{
    public class ScreenToAudioDevice : Dictionary<Screen, MMDevice>
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
