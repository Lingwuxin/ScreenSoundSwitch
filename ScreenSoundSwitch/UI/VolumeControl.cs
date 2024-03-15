using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace ScreenSoundSwitch
{
    public partial class VolumeControl : UserControl
    {
        MMDevice device;
        Screen screen;
        public VolumeControl()
        {
            InitializeComponent();
        }
        public void setDevice(MMDevice device)
        {
            if(this.device!=null&&this.device.ID.Equals(device.ID))
            {
                return;
            }
            this.device = device;
            setDeviceName();
            setVolume();
        }
        public string getScreenName()
        {
            return screen.DeviceName;
        }
        public void setScreen(Screen screen)
        {
            if(this.screen!=null&&this.screen.DeviceName.Equals(screen.DeviceName))
            {
                return;
            }
            Debug.WriteLine("set screen to " + screen.DeviceName);
            this.screen = screen;
            screenMsg.Text= screen.DeviceName;
        }
        private void setDeviceName()
        {
            deviceLable.Text = device.FriendlyName;
        }
        public int getVolume()
        {
            return volumeTraceBar.Value;
        }
        public void setVolume()
        {
            Debug.WriteLine("get " + device.FriendlyName + "volume" + device.AudioEndpointVolume.MasterVolumeLevelScalar * 10);
            volumeTraceBar.Value = (int)device.AudioEndpointVolume.MasterVolumeLevelScalar*10;
        }
        public void volumeTraceBar_ValueChanged(object? sender, EventArgs e)
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeTraceBar.Value / 10.0f;
            Debug.WriteLine("set " + device.FriendlyName + "volume to " + device.AudioEndpointVolume.MasterVolumeLevelScalar);
        }
        private void VolumeControl_Load(object sender, EventArgs e)
        {
            volumeTraceBar.ValueChanged += new EventHandler(volumeTraceBar_ValueChanged);
        }
    }
}
