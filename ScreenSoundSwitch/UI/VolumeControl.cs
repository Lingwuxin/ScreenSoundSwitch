using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Windows.Forms;

namespace ScreenSoundSwitch
{
    public partial class VolumeControl : UserControl
    {
        MMDevice device;
<<<<<<< HEAD
        Screen[] screens;
=======
>>>>>>> master
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
            float volume = device.AudioEndpointVolume.MasterVolumeLevelScalar * 10;
            setVolume((int)volume);
        }
<<<<<<< HEAD
        public void setScreen(Screen[]? screens)
        {
            if(screens==null||screens.Length==0)
            {
                screenMsg.Text = "no select";
                return;
            }
            this.screens = screens;
            screenMsg.Text = "";
            for(int i = 0; i < screens.Length; i++)
            {
                Debug.WriteLine("set screen to " + screens[i].DeviceName);
                screenMsg.Text += screens[i].DeviceName;
            }
            
            
        }
=======
>>>>>>> master
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
            Debug.WriteLine("set " + device.FriendlyName + "volume to " + value);
            volumeTraceBar.Value = value;
        }
        public void volumeTraceBar_ValueChanged(object? sender, EventArgs e)
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeTraceBar.Value / 10.0f;
            Debug.WriteLine("change " + device.FriendlyName + "volume to " + device.AudioEndpointVolume.MasterVolumeLevelScalar*10);
        }
        private void VolumeControl_Load(object sender, EventArgs e)
        {
            volumeTraceBar.ValueChanged += new EventHandler(volumeTraceBar_ValueChanged);
        }
    }
}
