using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Audio.inter
{
    [ComImport]
    [Guid("1C158861-B533-4B30-B1CF-E853E51C59B8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IChannelAudioVolume
    {
        [PreserveSig]
        int GetChannelCount(out uint channelCount);

        [PreserveSig]
        int SetChannelVolume(uint channelIndex, float volumeLevel, ref Guid eventContext);

        [PreserveSig]
        int GetChannelVolume(uint channelIndex, out float volumeLevel);

        [PreserveSig]
        int SetAllVolumes(uint channelCount, float[] volumes, ref Guid eventContext);

        [PreserveSig]
        int GetAllVolumes(uint channelCount, float[] volumes);
    }
}
