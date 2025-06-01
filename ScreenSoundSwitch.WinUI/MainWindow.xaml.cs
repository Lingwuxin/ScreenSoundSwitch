using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using ScreenSoundSwitch.WinUI.Views;
using SoundSwitch.Audio.Manager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        Dictionary<string, NavigationViewItem> navigationViewItems;
        public MainWindow()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
            this.Title = "ScreenSoundSwicth";
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 750));
            ExtendsContentIntoTitleBar = true;
            UserFrame.Navigate(typeof(LoginPage));
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
        private void GetAllMenuItems(IList<object> items)
        {            
            foreach (NavigationViewItem item in items)
            {
                navigationViewItems.Add(item.Tag.ToString(), item);
                if (item.MenuItems.Count != 0)
                {
                    GetAllMenuItems(item.MenuItems);
                }
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
                case "ShareFilePage":
                    navContentFrame.Navigate(typeof(ShareFilePage));
                    break;
                //case "PlayListPage":
                //    navContentFrame.Navigate(typeof(PlayListPage));
                //    break;
                //case "UserPage":
                //    navContentFrame.Navigate(typeof(UserPage));
                //    break;
                case "Settings":
                    navContentFrame.Navigate(typeof(SettingPage));
                    break; 

            }
        }

        private void NavigationViewItem_User_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
