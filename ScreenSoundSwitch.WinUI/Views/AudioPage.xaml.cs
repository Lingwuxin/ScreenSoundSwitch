using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ScreenSoundSwitch.WinUI.Models;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage;
using CommunityToolkit.WinUI.UI.Controls;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ScreenSoundSwitch.WinUI.ViewModels;
using Windows.Media.Core;
using Windows.Media.Playback;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class AudioPage : Page
    {
        AudioViewModel audioViewModel;
        AudioPlayerViewModel audioPlayerViewModel;
        public AudioPage()
        {
            this.InitializeComponent();
            audioPlayerViewModel = (AudioPlayerViewModel)this.DataContext;
            audioViewModel = (AudioViewModel) this.AudioListView.DataContext;
            //AudioMsgListView.ItemsSource = AudioFileMsg.GetContactsAsync();
        }

        private void ContentControl_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            AudioFileModel audioFileModel = (AudioFileModel)listView.SelectedItem;
            audioPlayerViewModel.AddAudioFlie(audioFileModel.File);

        }
    }
} 
