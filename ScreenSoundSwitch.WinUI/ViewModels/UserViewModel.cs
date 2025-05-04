using CommunityToolkit.Mvvm.ComponentModel;
using ScreenSoundSwitch.WinUI.Models;
using ScreenSoundSwitch.WinUI.Utils;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.ViewModels
{
    public partial class UserViewModel:ObservableObject
    {
        [ObservableProperty]
        public partial UserModel User { get; set; }
        private WebAPIHttpHelper webAPIHttpHelper = WebAPIHttpHelper.Instance;
        public UserViewModel()
        {
            User = new UserModel();
        }
        public async Task<UserModel> UserLogin(string username,string password)
        {
            User.Username = username;
            User.Password = password;
            await webAPIHttpHelper.Login(User);
            return User;
        }
        public async Task<UserModel> UserRegister(string username, string password)
        {
            User.Username = username;
            User.Password = password;
            await webAPIHttpHelper.Register(User);
            return User;
        }
        public async Task<UserModel> UserLogout()
        {
            webAPIHttpHelper.Logout();
            return User;
        }
    }
}
