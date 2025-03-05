using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace ScreenSoundSwitch.WinUI.Models
{
    public class AudioFileModel
    {
        public string Title { get; private set; }
        public string Author { get; private set; }
        public TimeSpan Duration { get; private set; }
        public StorageFile File { get; private set; }

        // 私有构造函数，防止外部直接调用
        private AudioFileModel(StorageFile storageFile)
        {
            File = storageFile;
            Title = storageFile.DisplayName; // 默认值
            Author = "Unknown";
            Duration = TimeSpan.Zero;
        }

        // 工厂方法（推荐使用）
        public static async Task<AudioFileModel> CreateAsync(StorageFile storageFile)
        {
            var audioFile = new AudioFileModel(storageFile);
            MusicProperties musicProperties = await storageFile.Properties.GetMusicPropertiesAsync();

            // 从 MusicProperties 获取详细信息
            audioFile.Title = string.IsNullOrEmpty(musicProperties.Title) ? storageFile.DisplayName : musicProperties.Title;
            audioFile.Author = string.IsNullOrEmpty(musicProperties.Artist) ? "Unknown" : musicProperties.Artist;
            audioFile.Duration = musicProperties.Duration;

            return audioFile;
        }
    }
}

