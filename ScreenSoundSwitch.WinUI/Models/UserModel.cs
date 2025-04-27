namespace ScreenSoundSwitch.WinUI.Models
{
    public partial class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserModel()
        {
            Username = "请输入用户名";
            Password = "请输入密码";
        }
    }
}
