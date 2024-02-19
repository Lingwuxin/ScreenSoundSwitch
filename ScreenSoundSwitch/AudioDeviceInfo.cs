
using NAudio.CoreAudioApi;

namespace ScreenSoundSwitch
{

    public class AudioDeviceEnumerator
    {
        public MMDeviceCollection GetDevices()
        {
            // 获取所有音频播放设备
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDeviceCollection devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            return devices;
            
        }
    }
}
