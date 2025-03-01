using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using ScreenSoundSwitch.WinUI.Models;

namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class ProcessControlViewModel: ObservableObject
    {
        [ObservableProperty]
        public partial  ProcessModel ProcessModel { get; set;}
        
        public ProcessControlViewModel()
        {
           ProcessModel = new ProcessModel();
        }
        internal void SetImage(BitmapImage image)
        {
            ProcessModel.Icon = image;
        }

        internal void SetProcessName(string processName)
        {
           ProcessModel.ProcessName = processName;
        }
    }
}
