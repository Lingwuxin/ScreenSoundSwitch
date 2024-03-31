using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch
{
     class DevicesConfig
    {
        public string FriendlyName { get; set; }
        public int MonitorIndex { get; set; }
        public int Volume { get; set; }
    }
     class WinformConfig
    {
        public bool isAutoStart = false;
    }
    internal class AppConfig
    {
        private WinformConfig winformConfig=new WinformConfig();
        private List<DevicesConfig> devicesConfig = new List<DevicesConfig>();
        string pathRoot = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+ "\\AppData\\Roaming\\ScreenSoundSwitch\\";
        public void AddDeviceConfig(string deviceName, int monitorIndex,int volume)
        {   
            if(!File.Exists(pathRoot)){
                Directory.CreateDirectory(pathRoot);
            }
            DevicesConfig deviceConfig = new DevicesConfig();
            deviceConfig.FriendlyName = deviceName;
            deviceConfig.MonitorIndex = monitorIndex;
            deviceConfig.Volume = volume;
            this.devicesConfig.Add(deviceConfig);
        }
        public void SetAutoStart(bool isAutoStart)
        {
            winformConfig.isAutoStart = isAutoStart;
        }
        public bool IsAutoStart()
        {
            return winformConfig.isAutoStart;
        }
        
        public List<DevicesConfig> GetDevicesConfig()
        {
            return devicesConfig;
        }
        //将配置保存为json格式文件
        public void WriteConfig()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(devicesConfig);
            File.WriteAllText(pathRoot+"DeviceConfig.json", json);
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(winformConfig);
            File.WriteAllText(pathRoot+"WinformConfig.json", json1);
        }
        public bool ReadWinformConfig()
        {
            if (File.Exists(pathRoot + "WinformConfig.json"))
            {
                string json = File.ReadAllText(pathRoot + "WinformConfig.json");
                try
                {
                    winformConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<WinformConfig>(json);
                }
                catch (JsonReaderException ex)
                {
                    Debug.WriteLine("JsonReaderException: " + ex.Message);
                    return false;
                }
                catch (JsonSerializationException ex)
                {
                    Debug.WriteLine("JsonSerializationException: " + ex.Message);
                    return false;
                }

                Debug.WriteLine(winformConfig);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ReadDeviceConfig()
        {
            if (File.Exists(pathRoot+"DeviceConfig.json"))
            {
                string json = File.ReadAllText(pathRoot+"DeviceConfig.json");
                try
                {
                    devicesConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DevicesConfig>>(json);
                }
                catch (JsonReaderException ex)
                {
                    Debug.WriteLine("JsonReaderException: " + ex.Message);
                    return false;
                }
                catch (JsonSerializationException ex)
                {
                    Debug.WriteLine("JsonSerializationException: " + ex.Message);
                    return false;
                }
                Debug.WriteLine(devicesConfig);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
