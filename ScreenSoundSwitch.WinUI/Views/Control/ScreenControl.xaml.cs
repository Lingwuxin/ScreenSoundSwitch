using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ScreenSoundSwitch.WinUI.ViewModels;
using System.Windows.Forms;
using Windows.UI;
using Application = Microsoft.UI.Xaml.Application;
using UserControl = Microsoft.UI.Xaml.Controls.UserControl;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{

    public sealed partial class ScreenControl : UserControl
    {
        public static readonly Microsoft.UI.Xaml.DependencyProperty ViewModelProperty =
            Microsoft.UI.Xaml.DependencyProperty.Register(
                "ViewModel", typeof(ScreenViewModel), typeof(ScreenControl), new Microsoft.UI.Xaml.PropertyMetadata(null));

        public ScreenViewModel ViewModel
        {
            get { return (ScreenViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly Microsoft.UI.Xaml.DependencyProperty ScreenProperty =
            Microsoft.UI.Xaml.DependencyProperty.Register(
                "Screen", typeof(Screen), typeof(ScreenControl), new Microsoft.UI.Xaml.PropertyMetadata(null));

        public Screen Screen
        {
            get { return (Screen)GetValue(ScreenProperty); }
            set { SetValue(ScreenProperty, value); }
        }

        public static readonly Microsoft.UI.Xaml.DependencyProperty ScaleProperty =
            Microsoft.UI.Xaml.DependencyProperty.Register(
                "Scale", typeof(double), typeof(ScreenControl), new Microsoft.UI.Xaml.PropertyMetadata(1.0));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set
            {
                SetValue(ScaleProperty, value);
                if (Screen != null)
                {
                    ScreenRect.Width = Screen.Bounds.Width * value;
                    ScreenRect.Height = Screen.Bounds.Height * value;
                }
            }
        }

        public static readonly Microsoft.UI.Xaml.DependencyProperty IsSelectedProperty =
            Microsoft.UI.Xaml.DependencyProperty.Register(
                "IsSelected", typeof(bool), typeof(ScreenControl), new Microsoft.UI.Xaml.PropertyMetadata(false, OnIsSelectedChanged));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void OnIsSelectedChanged(Microsoft.UI.Xaml.DependencyObject d, Microsoft.UI.Xaml.DependencyPropertyChangedEventArgs e)
        {
            var control = (ScreenControl)d;
            if ((bool)e.NewValue)
            {
                control.HighlightControl();
                if (control.ViewModel != null && control.Screen != null)
                {
                    control.ViewModel.SelectScreen(control.Screen);
                }
            }
            else
            {
                control.ResetControl();
            }
        }

        public static readonly Microsoft.UI.Xaml.DependencyProperty DeviceNameTextProperty =
            Microsoft.UI.Xaml.DependencyProperty.Register(
                "DeviceNameText", typeof(string), typeof(ScreenControl), new Microsoft.UI.Xaml.PropertyMetadata(string.Empty, OnDeviceNameTextChanged));

        public string DeviceNameText
        {
            get { return (string)GetValue(DeviceNameTextProperty); }
            set { SetValue(DeviceNameTextProperty, value); }
        }

        private static void OnDeviceNameTextChanged(Microsoft.UI.Xaml.DependencyObject d, Microsoft.UI.Xaml.DependencyPropertyChangedEventArgs e)
        {
            var control = (ScreenControl)d;
            control.DeviceName.Text = (string)e.NewValue;
        }

        public ScreenControl()
        {
            this.InitializeComponent();
        }
        private void HighlightControl()
        {
            Color heighLightColor = (Color)Application.Current.Resources["SystemAccentColorLight1"];
            ScreenRect.Fill = new SolidColorBrush(heighLightColor);
        }
        private void ResetControl()
        {
            ScreenRect.Fill = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
        }
        private void ScreenControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!IsSelected)
                ScreenRect.Fill = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColorDark1"]);
        }
        private void ScreenControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!IsSelected)
                ResetControl();
        }
        private void ScreenControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IsSelected = true;
            //通知ViewModel层选中了某个屏幕
        }
    }
}
