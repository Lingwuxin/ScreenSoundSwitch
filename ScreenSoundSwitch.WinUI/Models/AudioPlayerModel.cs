using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Models
{
    public class AudioPlayerModel
    {
        public string Title { get; set; }
        public string Path { get; set; }
        //记录已播放的时长
        public TimeSpan Duration { get; private set; }
    }
}
