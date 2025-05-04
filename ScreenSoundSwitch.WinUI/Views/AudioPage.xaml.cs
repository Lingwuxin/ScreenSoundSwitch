
using Microsoft.UI.Xaml.Controls;
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
            audioViewModel = (AudioViewModel) this.AudioListView.DataContext;
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
    }
} 
