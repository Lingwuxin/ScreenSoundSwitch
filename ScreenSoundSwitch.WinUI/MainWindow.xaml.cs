using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScreenSoundSwitch.WinUI.View;
using SoundSwitch.Audio.Manager;
using System.Diagnostics;
using Windows.Storage;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        
        //private SelectDevicePage selectDevicePage;
        //private VolumePage volumePage;
        //private ProcessPage processPage;
        //private AudioPage audioPage;
        //private SettingPage settingPage;
        ApplicationDataContainer localSettings;
        public MainWindow()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
            this.Title = "ScreenSoundSwicth";
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 750));
            ExtendsContentIntoTitleBar = true;
            //nav.SelectedItem = nav.MenuItems[0];
            //当窗口实例化完成后，初始化各个页面
            //navContentFrame.Navigate(typeof(SelectDevicePage));
        }


        private void NavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                string selectedTag = selectedItem.Tag.ToString();
                NavigateToPage(selectedTag);
            }
        }

        // 根据 Tag 导航到不同页面
        private void NavigateToPage(string pageTag)
        {
            switch (pageTag)
            {
                case "SelectDevicePage":
                    navContentFrame.Navigate(typeof(SelectDevicePage));
                    break;
                case "VolumePage":
                    navContentFrame.Navigate(typeof(VolumePage));
                    break;
                case "AudioPage":
                    navContentFrame.Navigate(typeof(AudioPage));
                    break;
                case "Settings":
                    navContentFrame.Navigate(typeof(SettingPage));
                    break; 
            }
        }

        private void AudioMediaTransportControls_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            Debug.WriteLine(e.Key);
        }
    }
}
