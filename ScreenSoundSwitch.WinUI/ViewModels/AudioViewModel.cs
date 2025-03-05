using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ScreenSoundSwitch.WinUI.Models;
using Windows.Storage;
using Windows.System;
using Microsoft.UI.Dispatching;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;

namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class AudioViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<AudioFileModel> audioFileModels = new();

        private readonly DispatcherQueue _dispatcherQueue;

        public AudioViewModel()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _dispatcherQueue.TryEnqueue(async () => await OpenAudioFolderAsync());
        }

        public async Task OpenAudioFolderAsync()
        {
            try
            {
                // 1. 获取音频文件夹路径
                object pathObj = ApplicationData.Current.LocalSettings.Values["AudioFilePath"];
                if (pathObj == null)
                {
                    Debug.WriteLine("Audio file path is not set.");
                    return;
                }

                string audioFilePath = pathObj.ToString();
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(audioFilePath);

                // 2. 获取音频文件
                var files = await folder.GetFilesAsync();
                foreach (StorageFile file in files)
                {
                    if (file.FileType == ".mp3" || file.FileType == ".wav" || file.FileType == ".wma")
                    {
                        AudioFileModel audioFile = await AudioFileModel.CreateAsync(file);                   
                        AudioFileModels.Add(audioFile);
                        Debug.WriteLine($"Loaded: {audioFile.Title}, {audioFile.Author}, {audioFile.Duration}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OpenAudioFolderAsync: {ex.Message}");
            }
        }
    }
}
