using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Media.Devices;

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
            if(this.device!=null&&this.device.Equals(device))
            {
                return;
            }
            this.device = device;
            setDeviceName();
            setVolume();
        }
        public void setScreen(Screen screen)
        {
            if(this.screen!=null&&this.screen.Equals(screen))
            {
                return;
            }
            this.screen = screen;
            screenNum.Text= screen.DeviceName;
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
            volumeTraceBar.Value = (int)device.AudioEndpointVolume.MasterVolumeLevelScalar;
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
