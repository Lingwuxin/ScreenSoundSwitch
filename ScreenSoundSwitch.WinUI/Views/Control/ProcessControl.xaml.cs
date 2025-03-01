using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using ScreenSoundSwitch.WinUI.ViewModels;
using SoundSwitch.Audio.Manager;
using SoundSwitch.Audio.Manager.Interop.Enum;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UserControl = Microsoft.UI.Xaml.Controls.UserControl;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    public sealed partial class ProcessControl : UserControl
    {
        private Process process;
        private Screen screen;
        private AudioSessionControl session;
        private bool sliderLock = false;
        private AudioSwitcher audioSwitcher;
        private ProcessControlViewModel viewModel;

        public ProcessControl(AudioSessionControl session)
        {
            this.InitializeComponent();
            viewModel=this.DataContext as ProcessControlViewModel;
            this.session = session;
            audioSwitcher = AudioSwitcher.Instance;
            SetProcess();
        }
        public int ProcessId
        {
            get { return process.Id; }
        }
        /// <summary>
        /// 该进程控件对应的进程所处的屏幕是否发生改变，如发生改变则更新screen属性
        /// </summary>
        /// <param name="screen"></param>
        /// <returns>screen==this.screen -> false</returns>
        public bool IsScreenChange(Screen screen)
        {
            if (this.screen.Equals(screen))
            {
                return false;
            }
            this.screen = screen;
            return true;
        }
        public void ChangeSimpleVolumeLevel(float level)
        {
            session.SimpleAudioVolume.Volume += level;
        }
        public void ChangeAudioDevice(MMDevice mMDevice)
        {
            audioSwitcher.SwitchProcessTo(mMDevice.ID, ERole.ERole_enum_count, EDataFlow.eRender, (uint)ProcessId);//ERole_enum_count，将该设备分配所有角色任务
        }

        private void SetProcess()
        {
            process = Process.GetProcessById((int)session.GetProcessID);
            viewModel.SetProcessName(process.ProcessName);
            screen = Screen.FromHandle(process.Handle);
            SimpleVolumeSlider.Value = session.SimpleAudioVolume.Volume * 100;
            var icon = Icon.ExtractAssociatedIcon(process.MainModule?.FileName);
            
            if (icon != null)
            {
                // 将Icon转换为BitmapImage
                BitmapImage bitmapImage = ConvertIconToBitmapImage(icon);

                viewModel.SetImage(bitmapImage);
                
            }
        }
        private BitmapImage ConvertIconToBitmapImage(Icon icon)
        {
            using (var stream = new MemoryStream())
            {
                icon.ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream.AsRandomAccessStream());
                return bitmapImage;
            }
        }

        private void SimpleVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (sliderLock)
            {
                return;
            }
            session.SimpleAudioVolume.Volume = (float)(e.NewValue / 100);
        }
    }
}
