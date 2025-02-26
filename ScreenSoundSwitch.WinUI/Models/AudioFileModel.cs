using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Models
{
    class AudioFileMsg
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public TimeSpan Duration { get; set; }

        public AudioFileMsg(string title, string author, TimeSpan duration)
        {
            Title = title;
            Author = author;
            Duration = duration;
        }
        //public static async Task<List<AudioFileMsg>> GetContactsAsync()
        //{
            
        //    return ;
        //}
    }
}

