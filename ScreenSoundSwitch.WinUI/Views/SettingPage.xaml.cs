using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
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
            ViewModel = this.DataContext as SettingViewModel;
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
                SettingStatusInfoBar.Severity = InfoBarSeverity.Success;
                SettingStatusInfoBar.Message = $"当前目录：{AudioFolderPathTextBlock.Text}";
            }
            else
            {
                SettingStatusInfoBar.Severity = InfoBarSeverity.Informational;
                SettingStatusInfoBar.Message = "尚未设置音频文件目录。";
            }

            EnableDebugToggleSwitch.IsOn = ViewModel.SettingModel.EnableDebugPage;
            var autoStartToggle = this.FindName("EnableAutoStartToggleSwitch") as ToggleSwitch;
            if (autoStartToggle != null)
            {
                autoStartToggle.IsOn = StartupManager.IsEnabled();
                ViewModel.SetEnableAutoStart(autoStartToggle.IsOn);
            }

            var trayToggle = this.FindName("EnableTrayIconToggleSwitch") as ToggleSwitch;
            if (trayToggle != null)
            {
                trayToggle.IsOn = ViewModel.SettingModel.EnableTrayIcon;
            }

            var autoRestoreToggle = this.FindName("EnableAutoRestoreConfigToggleSwitch") as ToggleSwitch;
            if (autoRestoreToggle != null)
            {
                autoRestoreToggle.IsOn = ViewModel.SettingModel.EnableAutoRestoreConfig;
            }

            var channelBalanceToggle = this.FindName("EnableScreenPositionChannelBalanceToggleSwitch") as ToggleSwitch;
            if (channelBalanceToggle != null)
            {
                channelBalanceToggle.IsOn = ViewModel.SettingModel.EnableScreenPositionChannelBalance;
            }

            var strengthSlider = this.FindName("ChannelBalanceStrengthSlider") as Slider;
            var strengthTextBlock = this.FindName("ChannelBalanceStrengthValueTextBlock") as TextBlock;
            if (strengthSlider != null)
            {
                strengthSlider.Value = ViewModel.SettingModel.ScreenPositionChannelBalanceStrength;
                if (strengthTextBlock != null)
                {
                    strengthTextBlock.Text = ((int)strengthSlider.Value).ToString();
                }
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

        private void EnableDebugToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var isEnabled = EnableDebugToggleSwitch.IsOn;
            ViewModel.SetEnableDebugPage(isEnabled);
            DebugPageState.SetEnabled(isEnabled);

            SettingStatusInfoBar.Severity = InfoBarSeverity.Informational;
            SettingStatusInfoBar.Message = isEnabled ? "已开启调试页面。" : "已关闭调试页面。";
        }

        private void EnableAutoStartToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = this.FindName("EnableAutoStartToggleSwitch") as ToggleSwitch;
            if (toggle == null)
            {
                return;
            }

            var isEnabled = toggle.IsOn;
            ViewModel.SetEnableAutoStart(isEnabled);
            StartupManager.SetEnabled(isEnabled);

            SettingStatusInfoBar.Severity = InfoBarSeverity.Informational;
            SettingStatusInfoBar.Message = isEnabled ? "已开启开机自启。" : "已关闭开机自启。";
        }

        private void EnableTrayIconToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = this.FindName("EnableTrayIconToggleSwitch") as ToggleSwitch;
            if (toggle == null)
            {
                return;
            }

            var isEnabled = toggle.IsOn;
            ViewModel.SetEnableTrayIcon(isEnabled);
            TrayIconState.SetEnabled(isEnabled);

            SettingStatusInfoBar.Severity = InfoBarSeverity.Informational;
            SettingStatusInfoBar.Message = isEnabled ? "已开启托盘模式。" : "已关闭托盘模式。";
        }

        private void EnableAutoRestoreConfigToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = this.FindName("EnableAutoRestoreConfigToggleSwitch") as ToggleSwitch;
            if (toggle == null)
            {
                return;
            }

            var isEnabled = toggle.IsOn;
            ViewModel.SetEnableAutoRestoreConfig(isEnabled);

            SettingStatusInfoBar.Severity = InfoBarSeverity.Informational;
            SettingStatusInfoBar.Message = isEnabled ? "已开启启动自动恢复配置入口。" : "已关闭启动自动恢复配置入口。";
        }

        private void EnableScreenPositionChannelBalanceToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var channelBalanceToggle = this.FindName("EnableScreenPositionChannelBalanceToggleSwitch") as ToggleSwitch;
            if (channelBalanceToggle == null)
            {
                return;
            }

            var isEnabled = channelBalanceToggle.IsOn;
            ViewModel.SetEnableScreenPositionChannelBalance(isEnabled);
            ChannelBalanceState.SetEnabled(isEnabled);

            SettingStatusInfoBar.Severity = InfoBarSeverity.Informational;
            SettingStatusInfoBar.Message = isEnabled
                ? "已开启按屏幕位置调整左右声道。"
                : "已关闭按屏幕位置调整左右声道。";
        }

        private void ChannelBalanceStrengthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var rounded = Math.Round(e.NewValue);
            var strengthTextBlock = this.FindName("ChannelBalanceStrengthValueTextBlock") as TextBlock;
            if (strengthTextBlock != null)
            {
                strengthTextBlock.Text = ((int)rounded).ToString();
            }
            ViewModel.SetScreenPositionChannelBalanceStrength(rounded);
        }
        /// <summary>
        /// 读取本地设置，并初始化控件状态
        /// </summary>
        private void LoadSettings()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            foreach (var key in localSettings.Values.Keys)
            {
                Debug.WriteLine(key);
            }
        }
    }
}
