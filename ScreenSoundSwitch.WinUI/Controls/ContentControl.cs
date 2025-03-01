using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Controls
{
    public sealed class ContentControl : Control
    {
        public ContentControl()
        {
            this.DefaultStyleKey = typeof(ContentControl);
        }


        // Í¼±êÊôÐÔ
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                "Icon",
                typeof(UIElement),
                typeof(ContentControl),
                new PropertyMetadata(null));

        public UIElement Icon
        {
            get => (UIElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        // ×ó²àÄÚÈÝÊôÐÔ
        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register(
                "LeftContent",
                typeof(object),
                typeof(ContentControl),
                new PropertyMetadata(null));

        public object LeftContent
        {
            get => GetValue(LeftContentProperty);
            set => SetValue(LeftContentProperty, value);
        }

        // ÓÒ²àÄÚÈÝÊôÐÔ
        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register(
                "RightContent",
                typeof(object),
                typeof(ContentControl),
                new PropertyMetadata(null));
       
        public object RightContent
        {
            get => GetValue(RightContentProperty);
            set => SetValue(RightContentProperty, value);
        }

    }
}
