 using Microsoft.UI;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VolumePage : Page
    {
        AudioDeviceManager audioDeviceManager;
        private MMDeviceViewModel MMDeviceViewModel => ((App)Application.Current).MMDeviceViewModel;
        public VolumePage()
        {
            this.InitializeComponent();
            audioDeviceManager = AudioDeviceManager.Instance;
            UpdateDevices();
        }
        private void UpdateDevices()
        {
            DevicesStackPanel.Children.Clear(); // 清空之前的内容
            var devices= audioDeviceManager.Devices;
            MMDeviceViewModel.ShareDate = devices;
            foreach (var device in devices) {
                AudioDeviceControl audioDeviceControl = new AudioDeviceControl(device);
                DevicesStackPanel.Children.Add(audioDeviceControl);
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // 检查传递的参数
            if (e.Parameter is bool refresh && refresh)
            {
                // 调用更新页面信息的方法
                UpdateDevices();
            }
        }
    }
}
