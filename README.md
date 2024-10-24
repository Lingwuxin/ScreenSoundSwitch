# ScreenSoundSwitch
此为master分支的说明
- A C#  playback device management application that enable to switches audio playback devices based on the screen where the process window is located.
- 一个能够根据进程窗口所在屏幕来切换播放设备的C#播放设备管理应用
## 环境
- OS: Windows 10
- Dev: .NET 8.0
## ReBuild分支
本次推送只为在远端进行备份，当前分支程序并不可用,如需使用请下载release发布的程序或选择master分支编译程序。
## 概述
旨在使用更为现代的WinUI3框架对项目进行重构，以期更美观更便捷的UI设计。在UI彻底完成重构之前，功能性内容将在本分支内完成。
## 关键功能的实现方式
- 切换播放设备的功能实现来自[SoundSwitch](https://github.com/Belphemur/SoundSwitch/tree/dev/SoundSwitch)
- 监听其他窗口活动通过Win32 API [setWinEventHook](https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-setwineventhook)，监听聚焦窗口是否发生切换的功能在SoundSwitch项目中已有封装,详见[WindowMonitor.cs](https://github.com/Belphemur/SoundSwitch/blob/dev/SoundSwitch.Audio.Manager/WindowMonitor.cs)，在此基础上添加了[ForegroundWindowMoved](https://github.com/Lingwuxin/ScreenSoundSwitch/blob/master/SoundSwitch.Audio.Manager/WindowMonitor.cs)事务委托，以便监听判断窗口是否移动到其他显示器的显示区域上。
### 开发中的功能
- <del>通过对NAudio库中的AudioSessionControl类型的继承并拓展IChannelAudioVolume相关接口，得到SAudioSessionControl类型，以便能够访问指定进程与音频终结点设备的会话，进而以进程为单位控制音频各通道的音量大小等细节。</del>
- 针对进程调整音量功能已完成，由于IChannelAudioVolume相关接口Windows api文档中并未给出具体的实例化方法，针对进程音频通道调整音量的功能暂无法继续进行。
- 拟实现通过监听当前聚焦的进程窗口通过快捷键调整进程音量的功能。
- ReBuild分支中根据窗口所在的显示器切换播放设备的功能暂未完成。
### 待实现功能
- 当设备信息变更时，根据当前的设备状态自动选择配置信息
- 期望能够实现一个软件混音器，允许指定进程同时使用多个播放设备
### 预览
![alt text](image-3.png)
获取正在使用播放设备的进程
![alt text](image-4.png)
