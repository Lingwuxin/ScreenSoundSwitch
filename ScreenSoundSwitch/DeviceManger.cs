using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch
{
    internal class DeviceManger
    {
        //通过屏幕设备名称记录对应的屏幕最近一次被选中的音频设备
        private Dictionary<string, string> screenNameToAudioDeviceName = new Dictionary<string, string>();
        private Dictionary<string, Screen[]> deviceNameToScreens = new Dictionary<string, Screen[]>();
        public void AddDevice(string deviceFriendlyName,Screen screen)
        {
            
            
            //如果最近一次被其他音频设备选中，则从其它设备中移除
            if (screenNameToAudioDeviceName.ContainsKey(screen.DeviceName))
            {
                string audioDeviceName = screenNameToAudioDeviceName[screen.DeviceName];
                for(int i = 0; i < deviceNameToScreens[audioDeviceName].Length; i++)
                {
                    if (deviceNameToScreens[audioDeviceName][i].DeviceName.Equals(screen.DeviceName))
                    {
                        Debug.WriteLine("remove screen " + screen.DeviceName + " of " + audioDeviceName);
                        List<Screen> screens= deviceNameToScreens[audioDeviceName].ToList();
                        screens.RemoveAt(i);
                        deviceNameToScreens[audioDeviceName]= screens.ToArray();
                        break;
                    }
                }
            }
            Debug.WriteLine("add screen " + screen.DeviceName + " of " + deviceFriendlyName);
            screenNameToAudioDeviceName[screen.DeviceName] = deviceFriendlyName;
            if (deviceNameToScreens.ContainsKey(deviceFriendlyName))
            {
                deviceNameToScreens[deviceFriendlyName].Append(screen);
            }
            else
            {
                deviceNameToScreens.Add(deviceFriendlyName, [screen]);
            }
        }
        public Screen[]? GetScreens(string deviceFriendlyName)
        {
            if(deviceNameToScreens.ContainsKey(deviceFriendlyName))
            {
                return deviceNameToScreens[deviceFriendlyName];
            }
            else
            {
                return null;
            }
        }
    }
}
