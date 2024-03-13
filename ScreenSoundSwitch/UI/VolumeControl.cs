using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public VolumeControl()
        {
            InitializeComponent();
        }
        public void setTitle(string title)
        {
            deviceLable.Text = title;
        }
        public int getVolume()
        {
            return volumeTraceBar.Value;
        }
        //从设备获取音量
        public void updateVolume()
        {
        }
        private void VolumeControl_Load(object sender, EventArgs e)
        {
            
        }
    }
}
