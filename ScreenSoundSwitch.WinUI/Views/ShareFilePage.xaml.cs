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
using ScreenSoundSwitch.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareFilePage : Page
    {
        AudioViewModel audioViewModel;
        AudioPlayerViewModel audioPlayerViewModel;
        public ShareFilePage()
        {
            this.InitializeComponent();
            audioPlayerViewModel = (AudioPlayerViewModel)this.DataContext;
            audioViewModel = (AudioViewModel)this.AudioListView.DataContext;

        }
        private void ContentControl_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            AudioFileModel audioFileModel = (AudioFileModel)listView.SelectedItem;
            audioPlayerViewModel.PlayListItem_DoubleTapped(audioFileModel);

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SearchBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void PlayMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void UploadMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DownloadMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DeletMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
