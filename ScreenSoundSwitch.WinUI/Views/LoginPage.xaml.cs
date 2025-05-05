using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScreenSoundSwitch.WinUI.ViewModels;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private UserViewModel _userViewModel;
        public LoginPage()
        {
            this.InitializeComponent();
            _userViewModel=this.DataContext as UserViewModel;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await _userViewModel.UserLogin(EmailInput.Text, PasswordInput.Password);
            if (_userViewModel.User.LoginStatus)
            {
                Frame.Navigate(typeof(UserPage));
            }
            else
            {
                Debug.WriteLine("Login failed");
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }
    }
}
