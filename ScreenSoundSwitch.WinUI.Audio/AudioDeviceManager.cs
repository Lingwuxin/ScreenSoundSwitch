
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using ScreenSoundSwitch.WinUI.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ScreenSoundSwitch
{

    public class AudioDeviceManger
    {
        Guid eventContext = Guid.NewGuid();
        public MMDeviceCollection GetDevices()
        {
            // 获取所有音频播放设备
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDeviceCollection devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            return devices;
            
        }
        public bool IsUsingAudioDeviceByProcessId(int processId,Dictionary<string,MMDevice?> deviceInfoDict)
        {            
            foreach (var deviceInfo in deviceInfoDict)
            {
                if (deviceInfo.Value.AudioSessionManager.Sessions == null)
                {
                    continue;
                }
                for (int i = deviceInfo.Value.AudioSessionManager.Sessions.Count; i > 0; i--)
                {
                    if (deviceInfo.Value.AudioSessionManager.Sessions[i - 1].State != AudioSessionState.AudioSessionStateActive)
                    {
                        continue;
                    }
                    if (deviceInfo.Value.AudioSessionManager.Sessions[i - 1].GetProcessID == 0)
                    {
                        continue;
                    }
                    if (deviceInfo.Value.AudioSessionManager.Sessions[i - 1].GetProcessID == processId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string? GetUsingAudioDeviceNameById(int processId, Dictionary<string, MMDevice?> deviceInfoDict)
        {
            foreach (var deviceInfo in deviceInfoDict)
            {
                if (deviceInfo.Value.AudioSessionManager.Sessions == null)
                {
                    continue;
                }
                for (int i = deviceInfo.Value.AudioSessionManager.Sessions.Count; i > 0; i--)
                {
                    if (deviceInfo.Value.AudioSessionManager.Sessions[i - 1].State != AudioSessionState.AudioSessionStateActive)
                    {
                        continue;
                    }
                    if (deviceInfo.Value.AudioSessionManager.Sessions[i - 1].GetProcessID == 0)
                    {
                        continue;
                    }
                    if (deviceInfo.Value.AudioSessionManager.Sessions[i - 1].GetProcessID == processId)
                    {
                        return deviceInfo.Key.ToString();
                    }
                }
            }
            return null;
        }
        static int GetAudioDeviceProcessId(MMDevice device)
        {
            if (device.AudioSessionManager.Sessions != null)
            {
                // 获取正在使用音频设备的进程 ID
                for (int i = 0; i < device.AudioSessionManager.Sessions.Count; i++)
                {
                    var session = device.AudioSessionManager.Sessions[i];
                    if (session.State == AudioSessionState.AudioSessionStateActive)
                    {
                        using (var process = Process.GetProcessById((int)session.GetProcessID))
                        {
                            return process.Id;
                        }
                    }
                }
            }
            return -1; // 如果没有进程正在使用音频设备，则返回 -1
        }
        public void SetProcessVolume(MMDeviceCollection devices,uint processId,float volume)
        {
            foreach (var device in devices) {
                SessionCollection sessions = device.AudioSessionManager.Sessions;
                if (sessions != null)
                {
                    for (int i = 0; i < sessions.Count; i++)
                    {
                        if (sessions[i].GetProcessID == processId)
                        {
                            SAudioSessionControl sessionControl = new SAudioSessionControl(sessions[i]);
                            sessionControl.SimpleAudioVolume.Volume = volume;
                        }
                    }
                }
            }
        }
    }
}
