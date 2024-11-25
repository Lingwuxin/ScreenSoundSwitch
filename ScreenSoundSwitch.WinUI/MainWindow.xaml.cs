using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.Models;
using ScreenSoundSwitch.WinUI.View;
using SoundSwitch.Audio.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window {
        WindowMonitor mindowMonitor;
        private SelectDevicePage selectDevicePage;
        private VolumePage volumePage;
        private ProcessPage processPage;
        ApplicationDataContainer localSettings;
       
        public MainWindow()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
            this.Title = "ScreenSoundSwicth";
            ExtendsContentIntoTitleBar = true;
            selectDevicePage = new SelectDevicePage();
            volumePage = new VolumePage();
            processPage = new ProcessPage();
            processPage.UpdateProcessBySeesion();
            mindowMonitor = new WindowMonitor();
            // 默认显示的页面
            navContentFrame.Content = selectDevicePage;
            mindowMonitor.ForegroundChanged += MindowMonitor_ForegroundChanged;
            mindowMonitor.MouseWheelScrolled += MindowMonitor_KeyIsDown;
            mindowMonitor.ForegroundWindowMoved += MindowMonitor_ForegroundMoved;
        }

        private void MindowMonitor_ForegroundMoved(object sender, WindowMonitor.Event e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                processPage.ForegroundMovedHandle(e.Hwnd,e.ProcessId);
            });
        }
        /// <summary>
        /// 监听聚焦窗口是否发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MindowMonitor_ForegroundChanged(object sender, WindowMonitor.Event e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                processPage.UpdataForegroundProcess(e.ProcessId);
            });
            
        }

        /// <summary>
        /// 监听Ctrl+Shift+鼠标滚轮，调整当前聚焦窗口的音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MindowMonitor_KeyIsDown(object sender, WindowMonitor.MouseWheelEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                Debug.WriteLine(e.Delta);
                processPage.UpdataForegroundVolume(e.Delta);
            });
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
            switch (pageTag)
            {
                case "SelectDevicePage":
                    navContentFrame.Content = selectDevicePage;
                    break;
                case "VolumePage":
                    navContentFrame.Content = volumePage;
                    break;
                case "ProcessPage":
                    processPage.UpdateProcessBySeesion();
                    navContentFrame.Content = processPage;
                    break;
            }
        }
    }
}
