using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace ScreenSoundSwitch
{
    public partial class VolumeControl : UserControl
    {
        MMDevice device;
        public VolumeControl()
        {
            InitializeComponent();
        }
        public void setDevice(MMDevice? device)
        {
            if(this.device!=null&&this.device.ID.Equals(device.ID))
            {
                Debug.WriteLine("设备获取失败！");
                return;
            }
            this.device = device;
            setDeviceName();
            setVolume((int)device.AudioEndpointVolume.MasterVolumeLevelScalar * 10);
        }
        private void setDeviceName()
        {
            deviceLable.Text = device.FriendlyName;
        }
        public int getVolume()
        {
            return volumeTraceBar.Value;
        }
        public void setVolume(int value)
        {
            Debug.WriteLine("set " + device.FriendlyName + "volume " + value);
            volumeTraceBar.Value = value;
        }
        public void volumeTraceBar_ValueChanged(object? sender, EventArgs e)
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeTraceBar.Value / 10.0f;
            Debug.WriteLine("set " + device.FriendlyName + "volume to " + device.AudioEndpointVolume.MasterVolumeLevelScalar*10);
        }
        private void VolumeControl_Load(object sender, EventArgs e)
        {
            volumeTraceBar.ValueChanged += new EventHandler(volumeTraceBar_ValueChanged);
        }
    }
}
