
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Diagnostics;

namespace ScreenSoundSwitch
{

    public class AudioDeviceManger
    {
        public MMDeviceCollection GetDevices()
        {
            // 获取所有音频播放设备
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDeviceCollection devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            return devices;
            
        }
        bool GetAudioDeviceAllProcessId(Dictionary<string,MMDevice?> deviceInfoDict,Dictionary<uint,ProcessInfo> processInfoDict)
        {
            List<int> processIdList=new List<int>();
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
                    //判断进程是否已经被添加到processInfoDict
                    if (processInfoDict.ContainsKey(deviceInfo.Value.AudioSessionManager.Sessions[i - 1].GetProcessID))
                    {
                        processInfoDict[deviceInfo.Value.AudioSessionManager.Sessions[i - 1].GetProcessID].IsUsing = true;
                        continue;
                    }
                    else
                    {
                        Process process = Process.GetProcessById((int)deviceInfo.Value.AudioSessionManager.Sessions[i - 1].GetProcessID);
                        ProcessInfo processInfo = new ProcessInfo();
                        processInfo.IsUsing = true;
                        processInfo.process = process;
                        processIdList.Add(process.Id);
                    }
                }
            }
            foreach(var processId in processIdList)
            {
                if(!processInfoDict.ContainsKey((uint)processId))
                {
                    processInfoDict.Remove((uint)processId);
                }
            }
            return true;
        }
        bool IsUsingAudioDeviceByProcessId(int processId,Dictionary<uint,MMDevice?> deviceInfoDict)
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

            }
}
