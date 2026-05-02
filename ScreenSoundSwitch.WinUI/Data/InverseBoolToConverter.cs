using Microsoft.UI.Xaml.Data;
using System;

namespace ScreenSoundSwitch.WinUI.Data
{
    class InverseBoolToConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
     value is bool b ? !b : value;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            value is bool b ? !b : value;
    }
}
