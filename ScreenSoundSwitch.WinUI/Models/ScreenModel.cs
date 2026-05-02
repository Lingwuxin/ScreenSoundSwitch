using CommunityToolkit.Mvvm.ComponentModel;
using ScreenSoundSwitch.WinUI.ViewModels;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenSoundSwitch.WinUI.Models
{
    public class ScreenModel
    {
        public int DeviceNum { get; }
        public double Width { get; }
        public double Height { get; }
        public double Left { get; }
        public double Top { get; }
        public ScreenModel(int deviceNum, Rectangle deviceBounds)
        {
            DeviceNum = deviceNum;
            Width = deviceBounds.Width / 10;
            Height = deviceBounds.Height / 10;
            Left = deviceBounds.Left / 10;
            Top = deviceBounds.Top / 10;
        }
    }
    public partial class ScreenControlModel : ObservableObject
    {
        public int DeviceNum { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        private double left;
        public double Left
        {
            get => left;
            set
            {
                if (SetProperty(ref left, value))
                {
                    OnPropertyChanged(nameof(Margin));
                }
            }
        }

        private double top;
        public double Top
        {
            get => top;
            set
            {
                if (SetProperty(ref top, value))
                {
                    OnPropertyChanged(nameof(Margin));
                }
            }
        }
        public string Name { get; set; }
        public string DeviceNameText { get; set; }

        public Microsoft.UI.Xaml.Thickness Margin => new Microsoft.UI.Xaml.Thickness(Left, Top, 0, 0);

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public Screen Screen { get; set; }

        public double Scale { get; set; }
        public ScreenViewModel ViewModel { get; set; }

        public ScreenControlModel(int deviceNum, Rectangle deviceBounds)
        {
            DeviceNum = deviceNum;
            Width = deviceBounds.Width;
            Height = deviceBounds.Height;
            Left = deviceBounds.Left;
            Top = deviceBounds.Top;
        }
    }
}
