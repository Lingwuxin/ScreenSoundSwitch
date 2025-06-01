using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;


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
            _userViewModel = App.SharedUserViewModel;
            this.DataContext = _userViewModel;
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(_userViewModel.User.Username);
            Frame.Navigate(typeof(LoginPage));
        }
    }
}
