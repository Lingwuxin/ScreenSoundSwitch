using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ScreenSoundSwitch.WinUI.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "ScreenSoundSwicth";
            ExtendsContentIntoTitleBar = true;
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
            Type pageType = null;

            switch (pageTag)
            {
                case "SelectDevicePage":
                    pageType = typeof(SelectDevicePage);  // 确保你已经创建了这些页面
                    break;
                case "VolumePage":
                    pageType = typeof(VolumePage);
                    break;
                case "ProcessPage":
                    pageType = typeof(ProcessPage);
                    break;
            }

            if (pageType != null && navContentFrame.CurrentSourcePageType != pageType)
            {
                navContentFrame.Navigate(pageType);
            }
        }
    }
}
