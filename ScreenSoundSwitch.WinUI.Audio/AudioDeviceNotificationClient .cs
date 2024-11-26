using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Audio
{
    internal class AudioDeviceNotificationClient : IMMNotificationClient
    {
        public void OnDeviceStateChanged(string deviceId, uint newState)
        {
            Console.WriteLine($"Device state changed: {deviceId}, New state: {newState}");
        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            Console.WriteLine($"Device added: {pwstrDeviceId}");
        }

        public void OnDeviceRemoved(string deviceId)
        {
            Console.WriteLine($"Device removed: {deviceId}");
        }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string pwstrDefaultDeviceId)
        {
            Console.WriteLine($"Default device changed: {pwstrDefaultDeviceId}, Flow: {flow}, Role: {role}");
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            Console.WriteLine($"Property value changed: {pwstrDeviceId}");
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            Debug.WriteLine("test");
        }
    }
}
