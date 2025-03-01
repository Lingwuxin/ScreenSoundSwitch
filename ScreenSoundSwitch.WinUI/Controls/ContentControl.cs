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
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(ContentControl),
                new PropertyMetadata("默认标题"));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // 图标属性
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
