using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using  System.Windows.Forms;
using Application = Microsoft.UI.Xaml.Application;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectDevicePage : Page
    {   
        public ProcessModel processModel => ((App)Application.Current).processModel;
        public SelectDevicePage()
        {
            this.InitializeComponent();
            UpdateScreenSelection();
        }
        public void UpdateScreenSelection()
        {
            foreach (var screen in Screen.AllScreens)
            {
                ScreenListView.Items.Add(screen.DeviceName);
            }
        }
        //待实现
        //选择SceenExpander中的选项，然后显示当前选择的屏幕所使用的播放设备以及可选的播放设备，当前使用设备高亮显示
        private void ScreenListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScreenListView.SelectedItem != null)
            {
                SceenExpander.Header = ScreenListView.SelectedItem.ToString();
                // 处理选择的显示器逻辑
                var selectedScreen = ScreenListView.SelectedItem.ToString();
                UpdateDeviceSelection(); 
                DeviceListView.Visibility = Visibility.Visible;
            }
        }

        public void UpdateDeviceSelection()
        {
            AudioDeviceManger audioDeviceManger = new AudioDeviceManger();
            var devices=audioDeviceManger.GetDevices();
            if (devices.Count == 0)
            {
                DeviceListView.Items.Add("Audio device is none!");
                return;
            }
            DeviceListView.Items.Clear();
            foreach (var device in audioDeviceManger.GetDevices()) {
                DeviceListView.Items.Add(new DeviceMsg(device.FriendlyName));
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //在选择设备后点击按钮确认,存储并提交到processModel
            throw new NotImplementedException();
        }
    }

}
