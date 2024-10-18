using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using ScreenSoundSwitch.WinUI.Audio.inter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Audio
{
    public class ChannelAudioVolume
    {
        readonly IChannelAudioVolume channelAudioVolumeInterface;
        public ChannelAudioVolume(AudioSessionControl sessionControl)
        {
            var sessionControlIUnknown = Marshal.GetIUnknownForObject((IAudioSessionControl)sessionControl.SimpleAudioVolume);
            channelAudioVolumeInterface = Marshal.GetTypedObjectForIUnknown(sessionControlIUnknown, typeof(IChannelAudioVolume)) as IChannelAudioVolume;
        }
        // 实现 GetAllVolumes 方法
        public int GetAllVolumes(uint channelCount, float[] volumes)
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            Marshal.ThrowExceptionForHR(channelAudioVolumeInterface.GetAllVolumes(channelCount, volumes));
            return volumes.Length;
        }

        // 实现 GetChannelCount 方法
        public uint GetChannelCount()
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            channelAudioVolumeInterface.GetChannelCount(out var channelCount);
            return channelCount;
        }

        // 实现 GetChannelVolume 方法
        public float GetChannelVolume(uint channelIndex)
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            channelAudioVolumeInterface.GetChannelVolume(channelIndex, out var volume);
            return volume;
        }

        // 实现 SetAllVolumes 方法
        public void SetAllVolumes(uint channelCount, float[] volumes, ref Guid eventContext)
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            Marshal.ThrowExceptionForHR(channelAudioVolumeInterface.SetAllVolumes(channelCount, volumes, eventContext));
        }

        // 实现 SetChannelVolume 方法
        public void SetChannelVolume(uint channelIndex, float volume, ref Guid eventContext)
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            Marshal.ThrowExceptionForHR(channelAudioVolumeInterface.SetChannelVolume(channelIndex, volume, eventContext));
        }
    }
}
