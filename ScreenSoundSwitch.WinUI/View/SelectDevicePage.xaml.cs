using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Foundation.Collections;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectDevicePage : Page
    {
        public SelectDevicePage()
        {
            this.InitializeComponent();
            UpdateScreenSelection();
            UpdateDeviceSelection();
        }
        public void UpdateScreenSelection()
        {
            foreach (var screen in Screen.AllScreens)
            {
                ScreenListView.Items.Add(screen.DeviceName);
            }
        }
        private void ScreenListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScreenListView.SelectedItem != null)
            {
                // 处理选择的显示器逻辑
                var selectedScreen = ScreenListView.SelectedItem.ToString();
                // 例如，你可以将选择的显示器显示在一个 TextBlock 中
                // MessageBox.Show($"Selected: {selectedScreen}");
            }
        }
        public void UpdateDeviceSelection()
        {
            AudioDeviceManger audioDeviceManger = new AudioDeviceManger();
            foreach (var device in audioDeviceManger.GetDevices()) {
                DeviceListView.Items.Add(new DeviceMsg(device.FriendlyName));
            }

        }
    }

}
