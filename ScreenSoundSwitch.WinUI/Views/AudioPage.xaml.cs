
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using ScreenSoundSwitch.WinUI.Models;
using ScreenSoundSwitch.WinUI.ViewModels;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
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
            audioViewModel = (AudioViewModel)this.AudioListView.DataContext;
            //AudioMsgListView.ItemsSource = AudioFileMsg.GetContactsAsync();
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
