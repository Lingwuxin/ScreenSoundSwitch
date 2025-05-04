using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using ScreenSoundSwitch.WinUI.Utils;


namespace ScreenSoundSwitch.WinUI.Models
{
    public partial class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool LoginStatus { get; set; }
        public BitmapImage Avatar { get; set; }

        public UserModel()
        {
            LoginStatus = false;            
            Avatar = new()
            {
                UriSource = new Uri("ms-appx:///Assets/DefaultUserAvatar.jpg")
            };

        }
    }
}
