using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Windowing;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.Views;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly TrayIconManager _trayIconManager = new TrayIconManager();
        private bool _isClosingFromTray;
        public MainWindow()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
            this.Title = "ScreenSoundSwicth";
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 750));
            var appIconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "TrayIcon.ico");
            if (File.Exists(appIconPath))
            {
                this.AppWindow.SetIcon(appIconPath);
            }
            ExtendsContentIntoTitleBar = true;

            var debugEnabled = localSettings.Values.ContainsKey("EnableDebugPage") &&
                               localSettings.Values["EnableDebugPage"] is bool enabled &&
                               enabled;
            DebugPageNavItem.Visibility = debugEnabled ? Visibility.Visible : Visibility.Collapsed;
            DebugPageState.SetEnabled(debugEnabled);
            DebugPageState.VisibilityChanged += DebugPageState_VisibilityChanged;

            var trayEnabled = localSettings.Values.ContainsKey("EnableTrayIcon") &&
                              localSettings.Values["EnableTrayIcon"] is bool trayOn &&
                              trayOn;
            TrayIconState.SetEnabled(trayEnabled);
            TrayIconState.EnabledChanged += TrayIconState_EnabledChanged;

            _trayIconManager.OpenRequested += TrayIconManager_OpenRequested;
            _trayIconManager.HideRequested += TrayIconManager_HideRequested;
            _trayIconManager.ExitRequested += TrayIconManager_ExitRequested;

            this.AppWindow.Closing += AppWindow_Closing;
            this.Closed += MainWindow_Closed;

            if (trayEnabled)
            {
                _trayIconManager.Enable();
            }

            //nav.SelectedItem = nav.MenuItems[0];
            //当窗口实例化完成后，初始化各个页面
            var autoRestoreEnabled = localSettings.Values.ContainsKey("EnableAutoRestoreConfig") &&
                                     localSettings.Values["EnableAutoRestoreConfig"] is bool restoreOn &&
                                     restoreOn;
            navContentFrame.Navigate(autoRestoreEnabled ? typeof(VolumePage) : typeof(SelectDevicePage));
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            this.AppWindow.Closing -= AppWindow_Closing;
            DebugPageState.VisibilityChanged -= DebugPageState_VisibilityChanged;
            TrayIconState.EnabledChanged -= TrayIconState_EnabledChanged;
            _trayIconManager.OpenRequested -= TrayIconManager_OpenRequested;
            _trayIconManager.HideRequested -= TrayIconManager_HideRequested;
            _trayIconManager.ExitRequested -= TrayIconManager_ExitRequested;
            this.Closed -= MainWindow_Closed;
            _trayIconManager.Dispose();
        }

        private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            var trayEnabled = localSettings.Values.ContainsKey("EnableTrayIcon") &&
                              localSettings.Values["EnableTrayIcon"] is bool trayOn &&
                              trayOn;

            if (!trayEnabled || _isClosingFromTray)
            {
                return;
            }

            args.Cancel = true;
            sender.Hide();
        }

        private void TrayIconState_EnabledChanged(bool isEnabled)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (isEnabled)
                {
                    _trayIconManager.Enable();
                }
                else
                {
                    _trayIconManager.Disable();
                }
            });
        }

        private void TrayIconManager_OpenRequested()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _isClosingFromTray = false;
                this.AppWindow.Show();
            });
        }

        private void TrayIconManager_HideRequested()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                this.AppWindow.Hide();
            });
        }

        private void TrayIconManager_ExitRequested()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _isClosingFromTray = true;
                this.AppWindow.Destroy();
            });
        }

        private void DebugPageState_VisibilityChanged(bool isEnabled)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                DebugPageNavItem.Visibility = isEnabled ? Visibility.Visible : Visibility.Collapsed;
            });
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
                case "DebugPage":
                    navContentFrame.Navigate(typeof(DebugPage));
                    break;
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
