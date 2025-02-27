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
        //��isSelected������Ϊtrueʱ�����ÿؼ���������֪ͨViewModel��ѡ����ĳ����Ļ
        //��isSelected������Ϊfalseʱ�����ÿؼ��ָ�ԭ״
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (_isSelected)
                    {
                        // ������ʾ�ؼ�
                        HighlightControl();
                        // ֪ͨViewModelѡ����ĳ����Ļ
                        ViewModel.SelectScreen(screen);
                    }
                    else
                    {
                        // �ָ��ؼ�ԭ״
                        ResetControl();
                    }
                }
            }
        }
        private bool _isSelected = false;
        public string DeviceNameText { get; set; }
        private ScreenViewModel ViewModel { get; set; }
        public Screen screen;
        public ScreenControl(Screen screen,ScreenViewModel viewModel,double scale)
        {
            this.InitializeComponent();
            this.screen=screen;
            ViewModel =viewModel;
            DeviceNameText = screen.DeviceName;
            DeviceName.Text=screen.DeviceName;
            ScreenRect.Width = screen.Bounds.Width* scale;
            ScreenRect.Height = screen.Bounds.Height* scale;
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
            //֪ͨViewModel��ѡ����ĳ����Ļ
        }
    } 
}
