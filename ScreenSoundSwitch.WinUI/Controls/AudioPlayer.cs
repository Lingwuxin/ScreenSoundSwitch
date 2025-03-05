using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Controls
{
    class AudioPlayer: Control
    {
        //音频播放器控件
        //应该实现播放音频文件、暂停、继续、停止等功能
        //支持传入AudioSource对象
        public AudioPlayer()
        {
            this.DefaultStyleKey = typeof(AudioPlayer);
        }

    }
}
