using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public IServiceProvider Services { get; }
        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
            DebugLogStore.Initialize();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<AudioDeviceManager>(provider => AudioDeviceManager.Instance);
            services.AddSingleton<ScreenToAudioDevice>(provider => ScreenToAudioDevice.Instance);
            services.AddTransient<ScreenViewModel>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                m_window = new MainWindow();
                m_window.Activate();
            }
            catch (Exception ex)
            {
                // 处理异常，例如记录日志或显示消息
                Console.WriteLine($"Error activating window: {ex.Message}");
            }

        }

        public static Window m_window;
    }
}
