# ScreenSoundSwitch
A C# application that switches audio playback devices based on the screen where the process window is located.
## 根据进程窗口所在的屏幕来切换播放设备
## 环境
- Windows 10 21H2以上版本
- .NET 8.0
## 功能实现
- 切换播放设备的功能实现来自[SoundSwitch](https://github.com/Belphemur/SoundSwitch/tree/dev/SoundSwitch)
- 监听其他窗口活动通过Win32 API [setWinEventHook](https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-setwineventhook)
## 特别注意
- 当音乐软件打开桌面歌词时，播放音频的线程有可能会被绑定在歌词上，此时拖动播放器窗口并不会触发设备切换，拖动歌词则可正常触发
## 已知问题
- 最小化到系统托盘后鼠标悬停在应用图标上无法显示应用名称
- 系统息屏后唤醒设备切换异常(初步确定是由其他设备休眠，如当显示器通过HDMI连接，同时音响设备连接在显示器上时，由于显示器休眠导致Windows对播放设备进行了自动切换)
## 应注意问题
- 在对主窗口程序解耦时，应注意**调用 SetWinEventHook 的客户端线程必须具有消息循环才能接收事件**
