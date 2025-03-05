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
namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class AudioPlayerViewModel:ObservableObject
    {
        [ObservableProperty]
        public partial AudioFileModel AudioFileModel { get; set; }
        public AudioPlayerViewModel()
        {
            
        }
        public void SetAudioPlayer(AudioFileModel audioFileModel)
        {
            AudioFileModel = audioFileModel;
        }
        [RelayCommand]
        private void NextTrack()
        {
            Debug.WriteLine("Next Track");
        }
        [RelayCommand]
        private void PreviousTrack()
        {
            Debug.WriteLine("Previous Track");
        }
    }
}
