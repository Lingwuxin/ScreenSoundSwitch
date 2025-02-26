using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Windows.Forms;
using UserControl = Microsoft.UI.Xaml.Controls.UserControl;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views.Control
{
    public sealed partial class ScreenControl : UserControl
    {

        public ScreenControl(Screen screen)
        {
            this.InitializeComponent();
            DeviceName.Text=screen.DeviceName;
            ScreenRect.Width = screen.Bounds.Width/10;
            ScreenRect.Height = screen.Bounds.Height/10;
        }
    }
}
