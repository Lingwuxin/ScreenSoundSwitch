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


namespace ScreenSoundSwitch.WinUI.ViewModels
{
    /// <summary>
    /// 屏幕视图模型
    /// </summary>
    partial class ScreenViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ScreenControl> elements = new();


        /// <summary>
        /// 实例化所有屏幕控件
        /// </summary>
        /// 当ScreenPage需要显示时，通过Screen.AllScreens 获取所有屏幕，并创建ScreenControl对象，添加到Elements中。
        /// 通过统计所有屏幕的宽度和高度，计算出每个屏幕的位置，并设置到ScreenControl中。
        public void InitializeElements()
        {
            Elements.Clear();
            if(Screen.AllScreens.Length == 0)
            {
                return;
            }
            foreach (var screen in Screen.AllScreens)
            {
                var rect = new ScreenControl(screen);
                Debug.WriteLine(screen.Bounds);
                Canvas.SetLeft(rect,screen.Bounds.X/10);
                Canvas.SetTop(rect, screen.Bounds.Y/10);
                Elements.Add(rect);
            }
        }

        [RelayCommand]
        private void AddElement()
        {
            InitializeElements();
            //var rect = new ScreenControl(Screen.PrimaryScreen);
            //Debug.WriteLine(Screen.PrimaryScreen.Bounds);
            //Canvas.SetLeft(rect, 50 * Elements.Count);
            //Elements.Add(rect);
        }

        [RelayCommand]
        private void RemoveLastElement()
        {
            if (Elements.Count > 0)
            {
                Elements.RemoveAt(Elements.Count - 1);
            }
        }

    }
}
