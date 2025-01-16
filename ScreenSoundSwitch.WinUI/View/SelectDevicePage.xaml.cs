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
        //ѡ��SceenExpander�е�ѡ�Ȼ����ʾ��ǰѡ�����Ļ��ʹ�õĲ����豸�Լ���ѡ�Ĳ����豸����ǰʹ���豸������ʾ
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
            
            //��ѡ���豸������ťȷ��,�洢���ύ��processModel
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
