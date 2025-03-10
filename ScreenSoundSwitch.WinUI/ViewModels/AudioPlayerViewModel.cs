using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenSoundSwitch.WinUI.Models;
using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class AudioPlayerViewModel:ObservableObject
    {

        [ObservableProperty]
        public partial MediaPlaybackList PlaybackList { get; set; }
        [ObservableProperty]
        public ObservableCollection<StorageFile> playListFiles=new();
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
        public void AddAudioFlie(StorageFile file)
        {        
            PlaybackList.Items.Add(new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file)));
            PlayListFiles.Add(file);
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
        //[RelayCommand]
        //private void PlaylistButton()
        //{
        //    IsPopupOpen = true;
        //    Debug.WriteLine("Open Playlist");
        //}
    }
}
