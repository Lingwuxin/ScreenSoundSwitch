using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace ScreenSoundSwitch
{
    public partial class VolumeControl : UserControl
    {
        MMDevice? device;
        public VolumeControl()
        {
            InitializeComponent();
        }
        private void VolumeControl_Load(object sender, EventArgs e)
        {
            volumeTraceBar.Scroll += new EventHandler(volumeTraceBar_Scroll);
        }
        public void volumeTraceBar_Scroll(object? sender, EventArgs e)
        {
            setVolume();
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
            setDeviceMsg();
            device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;
        }

        private void setDeviceMsg()
        {
            if (device == null)
            {
                return;
            }
            device.AudioEndpointVolume.OnVolumeNotification -= OnVolumeChange;
            volumeTraceBar.Value = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar*100);
            deviceLable.Text = device.FriendlyName;
            device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;
        }
        public int getVolume()
        {
            return volumeTraceBar.Value;
        }
        public void setVolume()
        {

            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeTraceBar.Value / 100.0f;
            Debug.WriteLine("set " + device.FriendlyName + "volume to " + device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        private void OnVolumeChange(AudioVolumeNotificationData data)
        {
            Debug.WriteLine($"{data.MasterVolume}");
            volumeTraceBar.Value = (int)(data.MasterVolume * 100);//data.MasterVolume * 100要加括号，否则会先将data.MasterVolume转换成整数再运算
        }

    }
}
