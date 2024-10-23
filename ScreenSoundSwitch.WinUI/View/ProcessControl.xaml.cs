using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using ScreenSoundSwitch.WinUI.Audio;
using NAudio.CoreAudioApi;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using NAudio.Wave;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    public sealed partial class ProcessControl : UserControl
    {
        private Process process;
        private AudioSessionControl session;
        private bool sliderLock=false;
        private MMDevice[] devices;

        public ProcessControl(AudioSessionControl session)
        {
            this.InitializeComponent();
            this.session = session;
            SetProcessAsync();
            
        }
        public int ProcessId
        {
            get { return process.Id; }
        }
        private void Expander_Expanded(Expander sender, ExpanderExpandingEventArgs args)
        {
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Debug.WriteLine($"{n}: {caps.ProductName}");
            }

        }
        //public ProcessControl(Process process)
        //{
        //    this.InitializeComponent();
        //    this.process = process;
        //    SetProcess();
        //}
        private async Task SetProcessAsync()
        {
            process = Process.GetProcessById((int)session.GetProcessID);
            ProcessName.Text=process.ProcessName;
            SimpleVolumeSlider.Value = session.SimpleAudioVolume.Volume*100;
            var icon = Icon.ExtractAssociatedIcon(process.MainModule?.FileName);
            if (icon != null)
            {
                // 将Icon转换为BitmapImage
                BitmapImage bitmapImage = await ConvertIconToBitmapImage(icon);

                // 将BitmapImage赋值给Image控件
                SessionIcon.Source = bitmapImage;
            }
        }
        private async Task<BitmapImage> ConvertIconToBitmapImage(Icon icon)
        {
            using (var stream = new MemoryStream())
            {
                icon.ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
                return bitmapImage;
            }
        }
        private void SimpleVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (sliderLock) {
                return;
            }
            session.SimpleAudioVolume.Volume = (float)(e.NewValue/100);
        }
        private void VolumeChanged(object sender, EventArgs e)
        {
            sliderLock = true;
            
            sliderLock=false;
        }
    }
}
