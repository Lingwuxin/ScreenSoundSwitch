using CommunityToolkit.Mvvm.ComponentModel;
using ScreenSoundSwitch.WinUI.Models;
using System;
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
            if (localSettings.Values.ContainsKey("EnableDebugPage"))
            {
                SettingModel.EnableDebugPage = (bool)localSettings.Values["EnableDebugPage"];
            }

            if (localSettings.Values.ContainsKey("EnableScreenPositionChannelBalance"))
            {
                SettingModel.EnableScreenPositionChannelBalance = (bool)localSettings.Values["EnableScreenPositionChannelBalance"];
            }

            if (localSettings.Values.ContainsKey("ScreenPositionChannelBalanceStrength"))
            {
                SettingModel.ScreenPositionChannelBalanceStrength = Convert.ToDouble(localSettings.Values["ScreenPositionChannelBalanceStrength"]);
            }
        }

        internal void SetAudioFileFolder(StorageFolder folder)
        {
            SettingModel.AudioFilePath = folder.Path;
            localSettings.Values["AudioFilePath"] = SettingModel.AudioFilePath;
        }

        internal void SetEnableDebugPage(bool enabled)
        {
            SettingModel.EnableDebugPage = enabled;
            localSettings.Values["EnableDebugPage"] = enabled;
        }

        internal void SetEnableScreenPositionChannelBalance(bool enabled)
        {
            SettingModel.EnableScreenPositionChannelBalance = enabled;
            localSettings.Values["EnableScreenPositionChannelBalance"] = enabled;
        }

        internal void SetScreenPositionChannelBalanceStrength(double strength)
        {
            SettingModel.ScreenPositionChannelBalanceStrength = strength;
            localSettings.Values["ScreenPositionChannelBalanceStrength"] = strength;
        }
    }
}
