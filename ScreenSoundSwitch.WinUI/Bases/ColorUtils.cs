using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ScreenSoundSwitch.WinUI.Bases
{

    public static class ColorUtils
    {
        public static Color GetHighlightColor(Color originalColor, double factor = 1.2)
        {
            // 增加亮度因子
            byte newR = (byte)Math.Min(originalColor.R * factor, 255);
            byte newG = (byte)Math.Min(originalColor.G * factor, 255);
            byte newB = (byte)Math.Min(originalColor.B * factor, 255);

            // 返回新的高亮颜色
            return Color.FromArgb(originalColor.A, newR, newG, newB);
        }
    }
}
