using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenSoundSwitch.WinUI.Audio
{
    public class ScreenManager
    {
        private static ScreenManager _Instance;
        private ScreenManager() { }
        public static ScreenManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ScreenManager();
                }
                return _Instance;
            }
        }
        public static Screen GetScreenByDeviceName(string _DeviceName)
        {
            Screen targetScreen=null;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.DeviceName == _DeviceName)
                {
                    targetScreen = screen;
                }
            }
            return targetScreen;
        }
    }
}
