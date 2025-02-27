using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ScreenSoundSwitch.WinUI.Bases;
using System.Windows.Forms;
using UserControl = Microsoft.UI.Xaml.Controls.UserControl;
using Windows.UI;
using Application = Microsoft.UI.Xaml.Application;
using ScreenSoundSwitch.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views.Control
{

    public sealed partial class ScreenControl : UserControl
    {
        public bool isSelected { get; set; }=false;
        public string DeviceNameText { get; set; }
        private ScreenViewModel ViewModel { get; set; }
        public ScreenControl(Screen screen,ScreenViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel=viewModel;
            DeviceNameText = screen.DeviceName;
            DeviceName.Text=screen.DeviceName;
            ScreenRect.Width = screen.Bounds.Width/10;
            ScreenRect.Height = screen.Bounds.Height/10;
        }
        private void ScreenControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // 使用系统预设的高亮色
            Color heighLightColor= ColorUtils.GetHighlightColor((Color)Application.Current.Resources["SystemAccentColor"]);
            ScreenRect.Fill = new SolidColorBrush(heighLightColor);
        }
        private void ScreenControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ScreenRect.Fill = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
        }
        private void ScreenControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isSelected = true;
            
            ViewModel.SelectScreen(DeviceNameText);
            //通知ViewModel层选中了某个屏幕
        }
    } 
}
