using Microsoft.UI.Xaml.Media.Imaging;
using System;
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
        public string FormattedDuration => Duration.ToString(@"mm\:ss");
        public StorageFile File { get; private set; }
        public BitmapImage? CoverImage { get; private set; } // 封面图
        public string Local {get{ return _local; } }
        private string _shared;
        private string _local;
        private bool _isLocal;
        public bool IsLocal 
        { 
            get { return _isLocal; } 
            set { 
                _isLocal = value;
                if (_isLocal)
                {
                    _local = "本地";
                }
                _local = "未下载";
            } 
        }
        public bool IsSelected { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsShared  { get; set; }


        private AudioFileModel(StorageFile storageFile)
        {
            File = storageFile;
            Title = storageFile.DisplayName;
            Author = "Unknown";
            Duration = TimeSpan.Zero;
            CoverImage = null;
        }

        public static async Task<AudioFileModel> CreateAsync(StorageFile storageFile)
        {
            var audioFile = new AudioFileModel(storageFile);
            MusicProperties musicProperties = await storageFile.Properties.GetMusicPropertiesAsync();

            // 读取音乐元数据
            audioFile.Title = string.IsNullOrEmpty(musicProperties.Title) ? storageFile.DisplayName : musicProperties.Title;
            audioFile.Author = string.IsNullOrEmpty(musicProperties.Artist) ? "Unknown" : musicProperties.Artist;
            audioFile.Duration = musicProperties.Duration;

            // 获取封面图
            audioFile.CoverImage = await GetAlbumCoverAsync(storageFile);

            return audioFile;
        }

        private static async Task<BitmapImage?> GetAlbumCoverAsync(StorageFile file)
        {
            try
            {
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale);
                if (thumbnail != null && thumbnail.Size > 0)
                {
                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(thumbnail);
                    return bitmap;
                }
            }
            catch (Exception)
            {
                // 忽略异常，返回 null
            }
            return null;
        }
    }
}
