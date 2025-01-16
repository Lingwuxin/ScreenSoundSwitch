using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using  System.Windows.Forms;
using ScreenSoundSwitch.WinUI.Audio;
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
        private ScreenToAudioDevice screenToAudioDevice;
        private AudioDeviceManager audioDeviceManager;
        private MMDeviceViewModel MMDeviceViewModel => ((App)Application.Current).MMDeviceViewModel;
        public SelectDevicePage()
        {
            this.InitializeComponent();
            screenToAudioDevice=ScreenToAudioDevice.Instance;
            audioDeviceManager=AudioDeviceManager.Instance;
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
        //选择SceenExpander中的选项，然后显示当前选择的屏幕所使用的播放设备以及可选的播放设备，当前使用设备高亮显示
        private void ScreenListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScreenListView.SelectedItem != null)
            {
                SceenExpander.Header = ScreenListView.SelectedItem;           
            }
        }

        public void UpdateDeviceSelection()
        {
            //var devices = MMDeviceViewModel.ShareDate;
            var devices = audioDeviceManager.Devices;
            if (devices.Count == 0)
            {
                DeviceListView.Items.Add("Audio device is none!");
                return;
            }
            DeviceListView.Items.Clear();
            foreach (var device in devices)
            {
                DeviceListView.Items.Add(device.FriendlyName);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BindedList.Visibility == Visibility.Collapsed) 
            {
                BindedList.Visibility = Visibility.Visible;
            }
            
            //在选择设备后点击按钮确认,存储并提交到processModel
            Screen selectedScreen = ScreenManager.GetScreenByDeviceName(ScreenListView.SelectedItem.ToString());
            MMDevice selectedMMDevice = audioDeviceManager.GetDeviceByFriendlyName(DeviceListView.SelectedItem.ToString());
            if (screenToAudioDevice.ContainsKey(selectedScreen))
            {
                screenToAudioDevice[selectedScreen] = selectedMMDevice;                
            }
            else
            {
                screenToAudioDevice.Add(selectedScreen, selectedMMDevice);               
            }
            BindedList.Items.Clear();
            foreach (var item in screenToAudioDevice) {
                BindedList.Items.Add(item.Key.ToString()+" + "+item.Value.ToString());
            }
            
        }
    }

}
