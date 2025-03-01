using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenSoundSwitch.WinUI.Models;
using Windows.Storage;

namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class SettingViewModel:ObservableObject
    {
        [ObservableProperty]
        public partial SettingModel SettingModel { get; set; }
        public SettingViewModel()
        {
            SettingModel = new SettingModel();
        }

        internal void SetAudioFileFolder(StorageFolder folder)
        {
            SettingModel.AudioFilePath = folder.Path;
        }
    }
}
