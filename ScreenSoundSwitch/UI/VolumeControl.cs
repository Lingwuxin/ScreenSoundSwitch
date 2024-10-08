using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScreenSoundSwitch
{
    public partial class VolumeControl : UserControl
    {
        private MMDevice? device;
        private AudioEndpointVolume audioEndpointVolume;
        private AudioEndpointVolumeChannel leftChannel;
        private AudioEndpointVolumeChannel rightChannel;
        private void VolumeControl_Load(object sender, EventArgs e)
        {
            volumeTrackBar.Scroll += new EventHandler(VolumeTrackBar_Scroll);
            leftTrackBar.Scroll += new EventHandler(LeftChannelTrackBar_Scroll);
            rightTrackBar.Scroll += new EventHandler(RightChannelTrackBar_Scroll);

        }
        private void VolumeTrackBar_Scroll(object? sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                audioEndpointVolume.MasterVolumeLevelScalar = volumeTrackBar.Value/100f;
            }));
        }
        private void LeftChannelTrackBar_Scroll(object? sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                leftChannel.VolumeLevelScalar = leftTrackBar.Value / 100f;
            }));
        }
        private void RightChannelTrackBar_Scroll(object? sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                rightChannel.VolumeLevelScalar = rightTrackBar.Value / 100f;
            }));
        }
        private void OnVolumeChange(AudioVolumeNotificationData data)
        {
            Invoke(new Action(() =>
            {
                volumeTrackBar.Value = (int)(data.MasterVolume * 100);//data.MasterVolume * 100要加括号，否则会先将data.MasterVolume转换成整数再运算
                if (leftChannel != null && rightChannel != null)
                {
                    leftTrackBar.Value = (int)(leftChannel.VolumeLevelScalar* 100);
                    rightTrackBar.Value = (int)(rightChannel.VolumeLevelScalar * 100);
                }
            }));
        }
        private void SetChannels()//设置声道
        {
            AudioEndpointVolumeChannels channels = audioEndpointVolume.Channels;
            if (channels.Count != 2)
            {
                leftTrackBar.Hide();
                rightTrackBar.Hide();
                return;
            }
            leftChannel = channels[0];
            rightChannel = channels[1];
            leftTrackBar.Value = (int)(leftChannel.VolumeLevelScalar * 100);
            rightTrackBar.Value = (int)(rightChannel.VolumeLevelScalar * 100);
        }

        private void SetDeviceMsg()
        {
            if (device == null)
            {
                return;
            }
            volumeTrackBar.Value = (int)(audioEndpointVolume.MasterVolumeLevelScalar * 100);
            deviceLable.Text = device.FriendlyName;
        }

        public VolumeControl()
        {
            InitializeComponent();
        }


        public void setDevice(MMDevice device)
        {
            if (device == null)
            {
                Debug.WriteLine("设备为空");
                return;
            }
            if (this.device!=null&&this.device.ID.Equals(device.ID))
            {
                Debug.WriteLine("设备已设置");
                return;
            }
            this.device = device;
            audioEndpointVolume=device.AudioEndpointVolume;
            SetChannels();
            SetDeviceMsg();
            audioEndpointVolume.OnVolumeNotification += OnVolumeChange;
        }
       
        public int getVolumeByBar()
        {
            return volumeTrackBar.Value;
        }



    }
}
