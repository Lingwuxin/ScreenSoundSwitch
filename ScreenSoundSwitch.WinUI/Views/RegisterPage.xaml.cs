using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        private UserViewModel _userViewModel;
        public RegisterPage()
        {
            this.InitializeComponent();
            _userViewModel = this.DataContext as UserViewModel;
        }
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var isSuccess=await _userViewModel.UserRegister(EmailInput.Text,UserNameInput.Text,PasswordInput.Password);
            if (isSuccess)
            {
                Frame.Navigate(typeof(LoginPage));
            }
            else {
                Debug.WriteLine("register faild");
            }
        }
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }
    }
}
