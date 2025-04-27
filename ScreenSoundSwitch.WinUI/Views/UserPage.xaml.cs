using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScreenSoundSwitch.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserPage : Page
    {
        private UserViewModel _userViewModel;
        public UserPage()
        {
            this.InitializeComponent();
            _userViewModel=this.DataContext as UserViewModel;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _userViewModel.UserLogin(UserNameInput.Text, PasswordInput.Password);
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            _userViewModel.UserRegister(UserNameInput.Text, PasswordInput.Password);
        }
    }
}
