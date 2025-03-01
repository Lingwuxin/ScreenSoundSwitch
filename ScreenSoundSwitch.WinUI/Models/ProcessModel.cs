using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Models
{
    public class ProcessModel
    {
        public BitmapImage Icon { get; set; }
        public string ProcessName { get; set; }
        public int Volume { get; set; }

    }
}
