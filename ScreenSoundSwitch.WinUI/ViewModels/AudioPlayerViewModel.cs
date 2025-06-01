using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class AudioPlayerViewModel:ObservableObject
    {

        [ObservableProperty]
        public partial MediaPlaybackList PlaybackList { get; set; }
        [ObservableProperty]
        public ObservableCollection<AudioFileModel> playListFiles=new();
        public AudioPlayerViewModel()
        {
            PlaybackList = new MediaPlaybackList();
            PlaybackList.AutoRepeatEnabled = true;
            PlayListFiles = [];
        }
        public void SetPlaybackList(MediaPlaybackList playbackList)
        {
            PlaybackList = playbackList;
            
        }
        async public void PlayListItem_DoubleTapped(AudioFileModel audioFileModel)
        {
            var mediaSource = MediaSource.CreateFromStorageFile(audioFileModel.File);
            var playbackItem = new MediaPlaybackItem(mediaSource);
            var props = playbackItem.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Music;
            props.MusicProperties.Title = audioFileModel.Title; // 可选
            props.MusicProperties.Artist = audioFileModel.Author; // 可选

            props.Thumbnail = RandomAccessStreamReference.CreateFromStream(await audioFileModel.File.GetThumbnailAsync(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale));


            // 应用显示属性
            playbackItem.ApplyDisplayProperties(props);
            // 添加到播放列表
            PlaybackList.Items.Add(playbackItem);
            PlayListFiles.Add(audioFileModel);
        }
        
        [RelayCommand]
        private void NextTrack()
        {
            PlaybackList.MoveNext();
        }
        [RelayCommand]
        private void PreviousTrack()
        {
            PlaybackList.MovePrevious();
        }
        [RelayCommand]
        private void Play_PlayList()
        {
        }
        [RelayCommand]
        private void Remove_PlayList()
        {

        }
        //[RelayCommand]
        //private void PlaylistButton()
        //{
        //    IsPopupOpen = true;
        //    Debug.WriteLine("Open Playlist");
        //}
    }
}
