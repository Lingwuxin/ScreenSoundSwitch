using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Models
{
    class ScreenModel
    {
        public int DeviceNum { get; }
        public double Width { get; }
        public double Height { get; }
        public double Left { get; }
        public double Top { get; }
        public ScreenModel(int deviceNum, Rectangle deviceBounds)
        {
            DeviceNum = deviceNum;
            Width = deviceBounds.Width/10;
            Height = deviceBounds.Height/10;
            Left = deviceBounds.Left/10;
            Top = deviceBounds.Top/10;
        }
    }
    class ScreenControlModel
    {
        public int DeviceNum { get; }
        public double Width { get; }
        public double Height { get; }
        public double Left { get; }
        public double Top { get; }
        public ScreenControlModel(int deviceNum, Rectangle deviceBounds)
        {
            DeviceNum = deviceNum;
            Width = deviceBounds.Width;
            Height = deviceBounds.Height;
            Left = deviceBounds.Left;
            Top = deviceBounds.Top;
        }
    }
}
