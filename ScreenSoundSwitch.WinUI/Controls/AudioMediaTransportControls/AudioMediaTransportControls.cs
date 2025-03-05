using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Controls
{
    public sealed class AudioMediaTransportControls : MediaTransportControls
    {
        public AudioMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(AudioMediaTransportControls);
            this.IsNextTrackButtonVisible = true;
            this.IsPreviousTrackButtonVisible = true;
            this.IsZoomButtonVisible = false;
           
        }
        //ControlTemplate÷–”–
        public static readonly DependencyProperty NextTrackCommandProperty =
            DependencyProperty.Register("NextTrackCommand", typeof(ICommand), typeof(AudioMediaTransportControls), new PropertyMetadata(null));

        public ICommand NextTrackCommand
        {
            get { return (ICommand)GetValue(NextTrackCommandProperty); }
            set { SetValue(NextTrackCommandProperty, value); }
        }

        public static readonly DependencyProperty PreviousTrackCommandProperty =
            DependencyProperty.Register("PreviousTrackCommand", typeof(ICommand), typeof(AudioMediaTransportControls), new PropertyMetadata(null));

        public ICommand PreviousTrackCommand
        {
            get { return (ICommand)GetValue(PreviousTrackCommandProperty); }
            set { SetValue(PreviousTrackCommandProperty, value); }
        }
    }
}
