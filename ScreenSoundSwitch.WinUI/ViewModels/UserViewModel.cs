using CommunityToolkit.Mvvm.ComponentModel;
using ScreenSoundSwitch.WinUI.Models;
using ScreenSoundSwitch.WinUI.Utils;
using System;
using System.Net.Http;
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
            if (res != null)
            {
                User.LoginStatus = true;
                User.Username = res.Username;
                return User;
            }
            else
            {
                User.LoginStatus = false;
                return null;
            }
            
        }
        public async Task<bool> UserRegister(string email,string username, string password)
        {
            User.Email = email;
            User.Password = password;
            User.Username = username;
            HttpResponseMessage res= await webAPIHttpHelper.Register(User);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                User.LoginStatus = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<UserModel> UserLogout()
        {
            webAPIHttpHelper.Logout();
            User.LoginStatus = false;
            return User;
        }
    }
}
