using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Forms;
using ScreenSoundSwitch.WinUI.Data;
using NAudio.CoreAudioApi;
using System.Linq;
using ListView = Microsoft.UI.Xaml.Controls.ListView;
using ComboBox = Microsoft.UI.Xaml.Controls.ComboBox;
using ScreenSoundSwitch.WinUI.Views;


namespace ScreenSoundSwitch.WinUI.ViewModels
{
    /// <summary>
    /// 屏幕视图模型
    /// </summary>
    public partial class ScreenViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ScreenControl> elements = new();
        [ObservableProperty]
        private MMDeviceCollection audioDevices;
        [ObservableProperty]
        public partial Screen SelectedScreen { get; set; }
        [ObservableProperty]
        public partial MMDevice SelectedAudioDevice { get; set; }
        private ScreenToAudioDevice screenToAudioDevice;
        private AudioDeviceManager audioDeviceManager;
        public ScreenViewModel()
        {
            audioDeviceManager = AudioDeviceManager.Instance;
            audioDevices = audioDeviceManager.Devices;
            screenToAudioDevice = ScreenToAudioDevice.Instance;
            InitializeElements();
        }

        /// <summary>
        /// 实例化所有屏幕控件
        /// </summary>
        /// 当ScreenPage需要显示时，通过Screen.AllScreens 获取所有屏幕，并创建ScreenControl对象，添加到Elements中。
        public void InitializeElements()
        {
            Screen[] screens = Screen.AllScreens;
            double width = screens.Max(e => e.Bounds.X + e.Bounds.Width);
            double height = screens.Max(e => e.Bounds.Y + e.Bounds.Height);

            Elements.Clear();
            if(Screen.AllScreens.Length == 0)
            {
                return;
            }

            //根据maxX，maxY和Canvas Width="650" Height="300"来确定缩放比例，且等比例缩放
            //double scaleX = 650 / width;
            //double scaleY = 300 / height;
            //上面这个是非等比例缩放，下面这个是等比例缩放
            double scale=Math.Min(650/width,300/height);
            double centerX = width / 2*scale;
            double centerY = height / 2*scale;
            //默认为每个显示器分配系统使用的音频设备
            
            foreach (var screen in Screen.AllScreens)
            {
                var rect = new ScreenControl(screen, this,scale);
                screenToAudioDevice.Add(screen, audioDeviceManager.GetDefaultAudioEndpoint());
                if (rect.screen.Primary)
                {
                    rect.IsSelected = true;
                }
                double x = screen.Bounds.X * scale - centerX;//当前显示器到中心点的距离
                double y = screen.Bounds.Y * scale - centerY;
                double sitX = x  + 325;
                double sitY = y  + 150;
                Canvas.SetLeft(rect, sitX);
                Canvas.SetTop(rect, sitY);

                Elements.Add(rect);
                
            }
        }
        public void SelectScreen(Screen screen)
        {
            SelectedScreen = screen;
            //遍历其他屏幕控件，将当前屏幕控件的选中状态设置为false
            foreach (var element in Elements)
            {
                if (!element.DeviceNameText.Equals(screen.DeviceName) )
                {
                    element.IsSelected = false;
                }
            }
            //根据当前选中的屏幕，获取对应的音频设备
            if (screenToAudioDevice.ContainsKey(screen) && screenToAudioDevice[screen]!= null)
            {
                AudioDeviceSelectionChanged(screenToAudioDevice[screen]);          
            }
        }
        public void AudioDeviceSelectionChanged(object sender)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is MMDevice mMDevice)
            {
                   SelectedAudioDevice = mMDevice;
                if (!screenToAudioDevice.ContainsKey(SelectedScreen))
                {
                    screenToAudioDevice.Add(SelectedScreen, mMDevice);
                }
                else
                {
                    screenToAudioDevice[SelectedScreen] = mMDevice;
                }
            }
            if (sender is MMDevice _mMDevice)
            {
                SelectedAudioDevice = _mMDevice;
                screenToAudioDevice[SelectedScreen] = _mMDevice;
            }
        }

    }
}
