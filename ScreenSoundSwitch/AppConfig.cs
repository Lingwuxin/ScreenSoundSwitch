using Newtonsoft.Json;
using System.Diagnostics;

namespace ScreenSoundSwitch
{
    class DevicesConfig
    {
        public string FriendlyName { get; set; }
        public string MonitorName { get; set; }
        public int Volume { get; set; }
    }
    class WinformConfig
    {
        public bool isAutoStart = false;//是否开机自启动
    }
    internal class AppConfig
    {
        private WinformConfig winformConfig = new WinformConfig();
        private List<DevicesConfig> devicesConfig = new List<DevicesConfig>();
        string pathRoot = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\ScreenSoundSwitch\\";
        public void AddDeviceConfig(string deviceName, string monitorName, int volume)
        {
            if (!File.Exists(pathRoot))
            {
                Directory.CreateDirectory(pathRoot);
            }
            DevicesConfig _deviceConfig = new DevicesConfig();
            _deviceConfig.FriendlyName = deviceName;
            _deviceConfig.MonitorName = monitorName;
            _deviceConfig.Volume = volume;
            for (int i = 0; i < devicesConfig.Count; i++)
            {
                if (devicesConfig[i].MonitorName == monitorName)//遍历设备配置列表如果已存在相同的设备名，则更新，否则遍历完毕后添加配置到列表
                {
                    devicesConfig[i] = _deviceConfig;
                    return;
                }
            }
            devicesConfig.Add(_deviceConfig);
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
            string json = JsonConvert.SerializeObject(devicesConfig);
            File.WriteAllText(pathRoot + "DeviceConfig.json", json);
            string json1 = JsonConvert.SerializeObject(winformConfig);
            File.WriteAllText(pathRoot + "WinformConfig.json", json1);
        }
        public bool ReadWinformConfig()
        {
            if (File.Exists(pathRoot + "WinformConfig.json"))
            {
                string json = File.ReadAllText(pathRoot + "WinformConfig.json");
                try
                {
                    winformConfig = JsonConvert.DeserializeObject<WinformConfig>(json);
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
            if (File.Exists(pathRoot + "DeviceConfig.json"))
            {
                string json = File.ReadAllText(pathRoot + "DeviceConfig.json");
                try
                {
                    devicesConfig = JsonConvert.DeserializeObject<List<DevicesConfig>>(json);
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
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
