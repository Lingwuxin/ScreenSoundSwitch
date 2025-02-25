using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SoundSwitch.Audio.Manager;
using SoundSwitch.Audio.Manager.Interop.Enum;
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

        public ProcessControl(AudioSessionControl session)
        {
            this.InitializeComponent();
            this.session = session;
            audioSwitcher = AudioSwitcher.Instance;
            SetProcess();
        }
        public int ProcessId
        {
            get { return process.Id; }
        }
        /// <summary>
        /// �ý��̿ؼ���Ӧ�Ľ�����������Ļ�Ƿ����ı䣬�緢���ı������screen����
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
            audioSwitcher.SwitchProcessTo(mMDevice.ID, ERole.ERole_enum_count, EDataFlow.eRender, (uint)ProcessId);//ERole_enum_count�������豸�������н�ɫ����
        }
        private void Expander_Expanded(Expander sender, ExpanderExpandingEventArgs args)
        {
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Debug.WriteLine($"{n}: {caps.ProductName}");
            }

        }
        private void SetProcess()
        {
            process = Process.GetProcessById((int)session.GetProcessID);
            screen = Screen.FromHandle(process.Handle);
            ProcessName.Text = process.ProcessName;
            SimpleVolumeSlider.Value = session.SimpleAudioVolume.Volume * 100;
            var icon = Icon.ExtractAssociatedIcon(process.MainModule?.FileName);
            if (icon != null)
            {
                // ��Iconת��ΪBitmapImage
                BitmapImage bitmapImage = ConvertIconToBitmapImage(icon);

                // ��BitmapImage��ֵ��Image�ؼ�
                SessionIcon.Source = bitmapImage;
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
