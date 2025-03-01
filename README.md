# ScreenSoundSwitch
- A C#  playback device management application that enable to switches audio playback devices based on the screen where the process window is located.
- 一个能够根据进程窗口所在屏幕来切换播放设备的C#播放设备管理应用
## 环境
- OS: Windows 10
- Dev: .NET 8.0
## 概述
该项目是为了在最新版本的windows上实现更便捷的音频播放设备管理器
## 功能与实现
- 切换播放设备的功能实现来自[SoundSwitch](https://github.com/Belphemur/SoundSwitch/tree/dev/SoundSwitch)
- 监听其他窗口活动通过Win32 API [setWinEventHook](https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-setwineventhook)，监听聚焦窗口是否发生切换的功能在SoundSwitch项目中已有封装,详见[WindowMonitor.cs](https://github.com/Belphemur/SoundSwitch/blob/dev/SoundSwitch.Audio.Manager/WindowMonitor.cs)，在此基础上添加了[ForegroundWindowMoved](https://github.com/Lingwuxin/ScreenSoundSwitch/blob/master/SoundSwitch.Audio.Manager/WindowMonitor.cs)事务委托，以便监听判断窗口是否移动到其他显示器的显示区域上。
### 可使用的功能
- 使用LCtrl+LAlt+鼠标滚轮可调节当前聚焦窗口相关的会话音量。
- 可为每个显示器指定一个音频播放设备，当需要播放音频的进程窗口在显示器之间移动时，会自动切换进程所使用的播放设备
### 开发中的功能
- <del>通过对NAudio库中的AudioSessionControl类型的继承并拓展IChannelAudioVolume相关接口，得到SAudioSessionControl类型，以便能够访问指定进程与音频终结点设备的会话，进而以进程为单位控制音频各通道的音量大小等细节。</del>针对进程调整音量功能已完成，由于IChannelAudioVolume相关接口Windows api文档中并未给出具体的实例化方法，针对进程音频通道调整音量的功能暂无法继续进行。
### 待实现功能
- 当设备信息变更时，根据当前的设备状态自动选择配置信息
- 期望能够实现一个软件混音器，允许指定进程同时使用多个播放设备
### 预览
同步系统设置中的显示器布局
![alt text](image-5.png)
获取正在使用播放设备的进程
![alt text](image-6.png)