using CommunityToolkit.Mvvm.ComponentModel;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Text;

namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class UserViewModel:ObservableObject
    {
        [ObservableProperty]
        public partial UserModel User { get; set; }
        public UserViewModel()
        {
            User = new UserModel();
        }
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        public void UserLogin(string username,string password)
        {
            User.Username = username;
            User.Password = HashPassword(password);
        }
        public void UserRegister(string username, string password)
        {
            User.Username = username;
            User.Password = HashPassword(password);
        }

    }
}
