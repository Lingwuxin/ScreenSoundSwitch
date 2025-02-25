namespace ScreenSoundSwitch.WinUI.Models
{
    public class DeviceMsg
    {
        public string DeviceName { get; set; }
        public DeviceMsg(string deviceName)
        {
            DeviceName = deviceName;
        }
    }
}
