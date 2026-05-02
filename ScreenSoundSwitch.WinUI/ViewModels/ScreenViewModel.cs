using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace ScreenSoundSwitch.WinUI.ViewModels
{
    /// <summary>
    /// 屏幕视图模型
    /// </summary>
    public partial class ScreenViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ScreenControlModel> elements = new();
        [ObservableProperty]
        private MMDeviceCollection audioDevices;
        [ObservableProperty]
        public partial Screen SelectedScreen { get; set; }
        [ObservableProperty]
        public partial MMDevice SelectedAudioDevice { get; set; }
        private ScreenToAudioDevice screenToAudioDevice;
        private AudioDeviceManager audioDeviceManager;
        public ScreenViewModel(AudioDeviceManager audioManager, ScreenToAudioDevice screenToDeviceMap)
        {
            audioDeviceManager = audioManager;
            audioDevices = audioDeviceManager.Devices;
            screenToAudioDevice = screenToDeviceMap;
            InitializeElements();
        }

        /// <summary>
        /// 实例化所有屏幕控件
        /// </summary>
        /// 当ScreenPage需要显示时，通过Screen.AllScreens 获取所有屏幕，并创建ScreenControl对象，添加到Elements中。
        public void InitializeElements()
        {
            Screen[] screens = Screen.AllScreens;
            double minX = screens.Min(e => e.Bounds.X);
            double minY = screens.Min(e => e.Bounds.Y);
            double width = screens.Max(e => e.Bounds.X + e.Bounds.Width) - minX;
            double height = screens.Max(e => e.Bounds.Y + e.Bounds.Height) - minY;

            Elements.Clear();
            if (Screen.AllScreens.Length == 0)
            {
                return;
            }

            //根据maxX，maxY和Canvas Width="650" Height="300"来确定缩放比例，且等比例缩放
            //double scaleX = 650 / width;
            //double scaleY = 300 / height;
            //上面这个是非等比例缩放，下面这个是等比例缩放
            double scale = Math.Min(650 / width, 300 / height);
            double centerX = width / 2 * scale;
            double centerY = height / 2 * scale;
            //默认为每个显示器分配系统使用的音频设备

            foreach (var screen in Screen.AllScreens)
            {
                //默认为每个显示器分配系统使用的音频设备
                screenToAudioDevice.Add(screen, audioDeviceManager.GetDefaultAudioEndpoint());

                double x = (screen.Bounds.X - minX) * scale - centerX;//当前显示器到中心点的距离
                double y = (screen.Bounds.Y - minY) * scale - centerY;
                double sitX = x + 325;
                double sitY = y + 150;

                var model = new ScreenControlModel(0, screen.Bounds)
                {
                    Name = screen.DeviceName,
                    DeviceNameText = screen.DeviceName,
                    Left = sitX,
                    Top = sitY,
                    Width = screen.Bounds.Width * scale,
                    Height = screen.Bounds.Height * scale,
                    IsSelected = screen.Primary,
                    Screen = screen,
                    Scale = scale,
                    ViewModel = this
                };

                Elements.Add(model);
            }
        }
        public void SelectScreen(Screen screen)
        {
            SelectedScreen = screen;
            //遍历其他屏幕控件，将当前屏幕控件的选中状态设置为false
            foreach (var element in Elements)
            {
                if (!element.DeviceNameText.Equals(screen.DeviceName))
                {
                    element.IsSelected = false;
                }
                else
                {
                    element.IsSelected = true;
                }
            }
            //根据当前选中的屏幕，获取对应的音频设备
            if (screenToAudioDevice.ContainsKey(screen) && screenToAudioDevice[screen] != null)
            {
                // Ensure reference equality by fetching from the existing audioDevices collection
                var deviceId = screenToAudioDevice[screen].ID;
                var matchedDevice = audioDevices.FirstOrDefault(d => d.ID == deviceId) ?? screenToAudioDevice[screen];
                SelectedAudioDevice = matchedDevice;
            }
        }

        partial void OnSelectedAudioDeviceChanged(MMDevice value)
        {
            // Whenever SelectedAudioDevice changes (either via code or UI Combobox selection),
            // update the mapping for the currently selected screen.
            if (SelectedScreen != null && value != null)
            {
                if (!screenToAudioDevice.ContainsKey(SelectedScreen))
                {
                    screenToAudioDevice.Add(SelectedScreen, value);
                }
                else
                {
                    screenToAudioDevice[SelectedScreen] = value;
                }
            }
        }

        public void AudioDeviceSelectionChanged(object sender)
        {
            // 兼容之前代码保留该空方法，实际逻辑已转移到 OnSelectedAudioDeviceChanged 中被动响应。
        }

    }
}
