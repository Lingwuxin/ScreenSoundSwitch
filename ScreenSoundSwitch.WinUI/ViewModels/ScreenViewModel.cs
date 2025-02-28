using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using System;
using Windows.UI;
using ScreenSoundSwitch.WinUI.Views.Control;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml.Linq;
using ScreenSoundSwitch.WinUI.Data;
using NAudio.CoreAudioApi;
using System.Linq;
using ListView = Microsoft.UI.Xaml.Controls.ListView;
using ComboBox = Microsoft.UI.Xaml.Controls.ComboBox;


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
        public partial string SelectDeviceTextBlockText { get; set; }="请选择屏幕";
        [ObservableProperty]
        public partial string AudioDeviceExpanderHeader { get; set; } = "No Audio device selected";
        private ScreenToAudioDevice screenToAudioDevice;
        private AudioDeviceManager audioDeviceManager;
        private Screen selectedScreen;      
        public ScreenViewModel()
        {
            InitializeElements();
            audioDeviceManager =AudioDeviceManager.Instance;
            audioDevices=audioDeviceManager.Devices;
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

            foreach (var screen in Screen.AllScreens)
            {
                var rect = new ScreenControl(screen, this,scale);
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
            SelectDeviceTextBlockText = screen.DeviceName;
            selectedScreen = screen;
            //遍历其他屏幕控件，将当前屏幕控件的选中状态设置为false
            foreach (var element in Elements)
            {
                if (!element.DeviceNameText.Equals(screen.DeviceName) )
                {
                    element.IsSelected = false;
                }
            }
        }
        public void AudioDeviceSelectionChanged(object sender)
        {
            Debug.WriteLine("AudioDeviceSelectionChanged");
            if (sender is ComboBox comboBox && comboBox.SelectedItem is MMDevice selectedDevice)
            {
                // 假设 Header 的名称是 "选择音频设备"
                // 替换为您的 Expander 名称
                   AudioDeviceExpanderHeader = selectedDevice.FriendlyName; // 修改 Header 为选中的设备名称
            }
        }

    }
}
