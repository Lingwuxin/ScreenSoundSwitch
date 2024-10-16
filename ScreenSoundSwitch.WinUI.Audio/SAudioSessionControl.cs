using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using ScreenSoundSwitch.WinUI.Audio.inter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Audio
{
    public class SAudioSessionControl : AudioSessionControl
    {
        readonly IChannelAudioVolume channelAudioVolumeInterface;

        public SAudioSessionControl(AudioSessionControl audioSessionControl) : base((IAudioSessionControl)audioSessionControl)
        {
            channelAudioVolumeInterface = (IChannelAudioVolume)audioSessionControl;
        }

        // ʵ�� GetAllVolumes ����
        public int GetAllVolumes(uint channelCount, float[] volumes)
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            Marshal.ThrowExceptionForHR(channelAudioVolumeInterface.GetAllVolumes(channelCount, volumes));
            return volumes.Length;
        }

        // ʵ�� GetChannelCount ����
        public uint GetChannelCount()
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            channelAudioVolumeInterface.GetChannelCount(out var channelCount);
            return channelCount;
        }

        // ʵ�� GetChannelVolume ����
        public float GetChannelVolume(uint channelIndex)
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            channelAudioVolumeInterface.GetChannelVolume(channelIndex, out var volume);
            return volume;
        }

        // ʵ�� SetAllVolumes ����
        public void SetAllVolumes(uint channelCount, float[] volumes, ref Guid eventContext)
        {
            if (channelAudioVolumeInterface == null)
            {
                throw new InvalidOperationException("Channel audio volume interface not available.");
            }

            Marshal.ThrowExceptionForHR(channelAudioVolumeInterface.SetAllVolumes(channelCount, volumes, eventContext));
        }

        // ʵ�� SetChannelVolume ����
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
