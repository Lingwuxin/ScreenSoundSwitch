namespace ScreenSoundSwitch.WinUI.Models
{
    public class SettingModel
    {
        public string AudioFilePath { get; set; } = "Path";
        public bool EnableDebugPage { get; set; }
        public bool EnableScreenPositionChannelBalance { get; set; }
        public double ScreenPositionChannelBalanceStrength { get; set; } = 20;
    }
}
