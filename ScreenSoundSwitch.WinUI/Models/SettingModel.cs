namespace ScreenSoundSwitch.WinUI.Models
{
    public class SettingModel
    {
        public string AudioFilePath { get; set; } = "Path";
        public bool EnableDebugPage { get; set; }
        public bool EnableAutoStart { get; set; }
        public bool EnableTrayIcon { get; set; }
        public bool EnableAutoRestoreConfig { get; set; } = true;
        public bool EnableScreenPositionChannelBalance { get; set; }
        public double ScreenPositionChannelBalanceStrength { get; set; } = 20;
    }
}
