using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Data
{
    class InverseBoolToConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
     value is bool b ? !b : value;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            value is bool b ? !b : value;
    }
}
