using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Linq;
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

        public bool ContainsScreen(Screen screen)
        {
            return Keys.Any(key => key.DeviceName == screen.DeviceName);
        }

        public bool TryGetDevice(Screen screen, out MMDevice device)
        {
            var matched = this.FirstOrDefault(item => item.Key.DeviceName == screen.DeviceName);
            device = matched.Value;
            return device != null;
        }

        public void SetDevice(Screen screen, MMDevice device)
        {
            var matchedScreen = Keys.FirstOrDefault(key => key.DeviceName == screen.DeviceName);
            if (matchedScreen != null)
            {
                this[matchedScreen] = device;
                return;
            }

            this[screen] = device;
        }
    }
}
