
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using ScreenSoundSwitch.WinUI.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ScreenSoundSwitch
{
    //单例类型
    public class AudioDeviceManager : IDisposable
    {
        private static AudioDeviceManager _instance;
        private readonly MMDeviceEnumerator _enumerator;
        private readonly AudioDeviceNotificationClient _notificationClient;
        private MMDeviceCollection _devices;
        private List<AudioSessionControl> _audioSessionControls;

        private AudioDeviceManager()
        {
            _enumerator = new MMDeviceEnumerator();
            _notificationClient = new AudioDeviceNotificationClient();
            _devices = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            _enumerator.RegisterEndpointNotificationCallback(_notificationClient);
        }

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static AudioDeviceManager Instance => _instance ??= new AudioDeviceManager();

        public MMDeviceCollection Devices => _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        public bool IsUsingAudioDeviceByProcessId(int processId, Dictionary<string, MMDevice?> deviceInfoDict)
        {
            foreach (var deviceInfo in deviceInfoDict)
            {
                if (deviceInfo.Value?.AudioSessionManager.Sessions == null)
                {
                    continue;
                }
                for (int i = deviceInfo.Value.AudioSessionManager.Sessions.Count; i > 0; i--)
                {
                    var session = deviceInfo.Value.AudioSessionManager.Sessions[i - 1];
                    if (session.State != AudioSessionState.AudioSessionStateActive || session.GetProcessID == 0)
                    {
                        continue;
                    }
                    if (session.GetProcessID == processId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public MMDevice GetDeviceByFriendlyName(string _FriendlyName)
        {
            MMDevice targetDevice = null;
            foreach (MMDevice device in Devices)
            {
                if (device.FriendlyName == _FriendlyName)
                {
                    targetDevice = device;
                    break;
                }
            }

            if (targetDevice != null)
            {
                Console.WriteLine($"Found MMDevice: {targetDevice.FriendlyName}");
            }
            else
            {
                Console.WriteLine("MMDevice not found.");
            }
            return targetDevice;
        }
        public string? GetUsingAudioDeviceNameById(int processId, Dictionary<string, MMDevice?> deviceInfoDict)
        {
            foreach (var deviceInfo in deviceInfoDict)
            {
                if (deviceInfo.Value?.AudioSessionManager.Sessions == null)
                {
                    continue;
                }
                for (int i = deviceInfo.Value.AudioSessionManager.Sessions.Count; i > 0; i--)
                {
                    var session = deviceInfo.Value.AudioSessionManager.Sessions[i - 1];
                    if (session.State != AudioSessionState.AudioSessionStateActive || session.GetProcessID == 0)
                    {
                        continue;
                    }
                    if (session.GetProcessID == processId)
                    {
                        return deviceInfo.Key;
                    }
                }
            }
            return null;
        }

        public static int GetAudioDeviceProcessId(MMDevice device)
        {
            if (device.AudioSessionManager.Sessions != null)
            {
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
            return -1;
        }

        public List<AudioSessionControl> GetSessions()
        {
            _audioSessionControls ??= new List<AudioSessionControl>();
            _audioSessionControls.Clear();

            foreach (var device in Devices)
            {
                var sessions = device.AudioSessionManager.Sessions;
                if (sessions != null)
                {
                    for (int i = 0; i < sessions.Count; i++)
                    {
                        if (!sessions[i].IsSystemSoundsSession)
                        {
                            _audioSessionControls.Add(sessions[i]);
                        }
                    }
                }
            }
            return _audioSessionControls;
        }

        public void SetProcessVolume(MMDeviceCollection devices, uint processId, float volume)
        {
            foreach (var device in devices)
            {
                var sessions = device.AudioSessionManager.Sessions;
                if (sessions != null)
                {
                    for (int i = 0; i < sessions.Count; i++)
                    {
                        if (sessions[i].GetProcessID == processId)
                        {
                            sessions[i].SimpleAudioVolume.Volume = volume;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _enumerator.UnregisterEndpointNotificationCallback(_notificationClient);
            _enumerator.Dispose();
        }
    }
}
