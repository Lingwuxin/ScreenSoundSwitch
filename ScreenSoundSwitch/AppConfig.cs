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
        private Dictionary<string, DevicesConfig> devicesConfig = new Dictionary<string, DevicesConfig>();
        private Dictionary<string,bool> deviceIsExist = new Dictionary<string, bool>();
        string pathRoot = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+ "\\AppData\\Roaming\\ScreenSoundSwitch\\";
        public AppConfig()
        {
            if (!File.Exists(pathRoot))
            {
                Directory.CreateDirectory(pathRoot);
            }
        }
        public void AddDeviceConfig(string deviceName, int monitorIndex,int volume)
        {

            DevicesConfig deviceConfig = new DevicesConfig();
            deviceConfig.FriendlyName = deviceName;
            deviceConfig.MonitorIndex = monitorIndex;
            deviceConfig.Volume = volume;
            devicesConfig[deviceName]= deviceConfig;
        }
        public void SetAutoStart(bool isAutoStart)
        {
            winformConfig.isAutoStart = isAutoStart;
        }
        public bool IsAutoStart()
        {
            return winformConfig.isAutoStart;
        }
        
        public Dictionary<string, DevicesConfig> GetDevicesConfig()
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
            if (File.Exists(pathRoot+"WinformConfig.json"))
            {
                string json = File.ReadAllText(pathRoot + "WinformConfig.json");
                try
                {
                    winformConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<WinformConfig>(json);
                }
                catch(JsonReaderException ex)
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
                devicesConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DevicesConfig>>(json);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
