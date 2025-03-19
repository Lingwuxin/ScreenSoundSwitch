using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Models
{
    //采用单例模式设计，便于全局访问
    internal class UserModel
    {
        string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        string Token { get; set; }

        public static UserModel Instance { get; } = new UserModel();
    }
}
