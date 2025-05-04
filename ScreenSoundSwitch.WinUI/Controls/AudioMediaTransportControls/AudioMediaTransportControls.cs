using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Controls
{
    [WindowsRuntimeHelperType(typeof(AudioMediaTransportControls))]
    public sealed class AudioMediaTransportControls : MediaTransportControls
    {
        public AudioMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(AudioMediaTransportControls);
            this.IsNextTrackButtonVisible = true;
            this.IsPreviousTrackButtonVisible = true;
            this.IsZoomButtonVisible = false;           
        }
        //播放列表按钮的显示属性
        public static readonly DependencyProperty IsPlaylistButtonVisibleProperty =
            DependencyProperty.Register("IsPlaylistButtonVisible", typeof(bool), typeof(AudioMediaTransportControls), new PropertyMetadata(true));

        public bool IsPlaylistButtonVisible
        {
            get { return (bool)GetValue(IsPlaylistButtonVisibleProperty); }
            set { SetValue(IsPlaylistButtonVisibleProperty, value); }
        }
        //添加设置播放列表MediaPlaybackList 
        public static readonly DependencyProperty MediaPlayListProperty =
            DependencyProperty.Register("MediaPlayList", typeof(ObservableCollection<AudioFileModel>), typeof(AudioMediaTransportControls), new PropertyMetadata(null));

        public ObservableCollection<AudioFileModel> MediaPlayList
        {
            get { return (ObservableCollection<AudioFileModel>)GetValue(MediaPlayListProperty); }
            set { SetValue(MediaPlayListProperty, value); }
        }


        //播放列表按钮的点击命令属性
        public static readonly DependencyProperty PlaylistButtonCommandProperty =
            DependencyProperty.Register("PlaylistButtonCommand", typeof(ICommand), typeof(AudioMediaTransportControls), new PropertyMetadata(null));

        public ICommand PlaylistButtonCommand
        {
            get { return (ICommand)GetValue(PlaylistButtonCommandProperty); }
            set { SetValue(PlaylistButtonCommandProperty, value); }
        }

        //ControlTemplate中有
        public static readonly DependencyProperty NextTrackCommandProperty =
            DependencyProperty.Register("NextTrackCommand", typeof(ICommand), typeof(AudioMediaTransportControls), new PropertyMetadata(null));
       
        public ICommand NextTrackCommand
        {
            get { return (ICommand)GetValue(NextTrackCommandProperty); }
            set { SetValue(NextTrackCommandProperty, value); }
        }

        public static readonly DependencyProperty PreviousTrackCommandProperty =
            DependencyProperty.Register("PreviousTrackCommand", typeof(ICommand), typeof(AudioMediaTransportControls), new PropertyMetadata(null));

        public ICommand PreviousTrackCommand
        {
            get { return (ICommand)GetValue(PreviousTrackCommandProperty); }
            set { SetValue(PreviousTrackCommandProperty, value); }
        }

    }
}
