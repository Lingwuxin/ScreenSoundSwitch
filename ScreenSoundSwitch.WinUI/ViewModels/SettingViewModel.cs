using CommunityToolkit.Mvvm.ComponentModel;
using ScreenSoundSwitch.WinUI.Models;
using Windows.Storage;

namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial SettingModel SettingModel { get; set; }
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public SettingViewModel()
        {
            SettingModel = new SettingModel();
        }

        internal void SetAudioFileFolder(StorageFolder folder)
        {
            SettingModel.AudioFilePath = folder.Path;
            localSettings.Values["AudioFilePath"] = SettingModel.AudioFilePath;
        }
    }
}
