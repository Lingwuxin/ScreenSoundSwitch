using Microsoft.UI.Xaml.Controls;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;


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
            screenToAudioDevice = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<ScreenToAudioDevice>(App.Current.Services);
            audioDeviceManager = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AudioDeviceManager>(App.Current.Services);

            // Assign the bound DataContext to the DI resolved instance
            this.DataContext = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<ScreenViewModel>(App.Current.Services);
            screenViewModel = this.DataContext as ScreenViewModel;

            //Canvas canvas = ScreenItemsControl.ItemsPanelRoot as Canvas;
            //UpdateScreenSelection();
            //UpdateDeviceSelection(); 
        }
        private void OnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            screenViewModel.InitializeElements();
        }

    }

}
