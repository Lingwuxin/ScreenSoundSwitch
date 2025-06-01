using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;
using Application = Microsoft.UI.Xaml.Application;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectDevicePage : Page
    {
        private ScreenToAudioDevice screenToAudioDevice;
        private AudioDeviceManager audioDeviceManager;
       ScreenViewModel screenViewModel;
        public SelectDevicePage()
        {
            
            this.InitializeComponent();
            screenToAudioDevice = ScreenToAudioDevice.Instance;
            audioDeviceManager = AudioDeviceManager.Instance;
            screenViewModel=this.DataContext as ScreenViewModel;
           
            //Canvas canvas = ScreenItemsControl.ItemsPanelRoot as Canvas;
            //UpdateScreenSelection();
            //UpdateDeviceSelection(); 
        }
        private void OnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            screenViewModel.InitializeElements();
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            screenViewModel.AudioDeviceSelectionChanged(sender);
        }

    }

}
