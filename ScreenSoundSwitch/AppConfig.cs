﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch
{
     class DevicesConfig
    {
        public string DeviceName { get; set; }
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
        public void AddDeviceConfig(string deviceName, int monitorIndex,int volume)
        {   
            DevicesConfig devicesConfig = new DevicesConfig();
            devicesConfig.DeviceName = deviceName;
            devicesConfig.MonitorIndex = monitorIndex;
            devicesConfig.Volume = volume;
            this.devicesConfig.Add(devicesConfig);
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
            File.WriteAllText("DeviceConfig.json", json);
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(winformConfig);
            File.WriteAllText("WinformConfig.json", json1);
        }
        public bool ReadWinformConfig()
        {
            if (File.Exists("WinformConfig.json"))
            {
                string json = File.ReadAllText("WinformConfig.json");
                winformConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<WinformConfig>(json);
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
            if (File.Exists("DeviceConfig.json"))
            {
                string json = File.ReadAllText("DeviceConfig.json");
                devicesConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DevicesConfig>>(json);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
