# ScreenSoundSwitch
A C# application that switches audio playback devices based on the screen where the process window is located.
## 根据进程窗口所在的屏幕来切换播放设备
## 播放设备切换的功能的实现来自[SoundSwitch](https://github.com/Belphemur/SoundSwitch/tree/dev/SoundSwitch.Audio.Manager)
## 已知问题
- 在关闭正在使用播放设备的窗口时，有概率无法正确获取窗口坐标或行为
- 创建进程窗口改变时，没有判断进程是否正在使用播放设备（终结点设备），考虑检测进程与播放设备间是否存在会话来判断进程是否正在使用播放设备
