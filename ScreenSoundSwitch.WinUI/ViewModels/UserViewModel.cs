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
        public async Task<UserModel> UserLogin(string email,string password)
        {
            User.Email = email;
            User.Password = password;
            var res=await webAPIHttpHelper.Login(User);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                User.LoginStatus = true;
                
            }
            return User;
        }
        public async Task<UserModel> UserRegister(string email, string password)
        {
            User.Email = email;
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
