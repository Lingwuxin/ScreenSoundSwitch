using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage;
using ScreenSoundSwitch.WinUI.ViewModels;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        //存储设置信息
        private ApplicationDataContainer localSettings;
        private readonly SettingViewModel ViewModel;
        public SettingPage()
        {
            this.InitializeComponent();
            ViewModel =this.DataContext as SettingViewModel;
            Page_Loaded(this, null);
        }
        //加载配置localSettings.Values["AudioFilePath"]
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            if (localSettings.Values.ContainsKey("AudioFilePath"))
            {
                ViewModel.SetAudioFileFolder(StorageFolder.GetFolderFromPathAsync(localSettings.Values["AudioFilePath"].ToString()).AsTask().Result);
                AudioFolderPathTextBlock.Text = localSettings.Values["AudioFilePath"].ToString();
            }
        }
        private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        {
            //disable the button to avoid double-clicking
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            // Create a folder picker
            FolderPicker openPicker = new FolderPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.


            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your folder picker
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a folder
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedAudioFolderToken", folder);
                ViewModel.SetAudioFileFolder(folder);
                //本应该在Viewmodel中同步，但是由于未知原因并不能同步
                AudioFolderPathTextBlock.Text = folder.Path;
            }
            //re-enable the button
            senderButton.IsEnabled = true;
        }
        /// <summary>
        /// 读取本地设置，并初始化控件状态
        /// </summary>
        private void LoadSettings()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            foreach(var key in localSettings.Values.Keys)
            {
                Debug.WriteLine(key);
            }
        }
    }
}
