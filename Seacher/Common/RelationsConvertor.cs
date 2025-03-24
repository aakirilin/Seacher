using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Seacher.Models;


namespace Seacher.Common
{
    public class RelationsConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetName((Relations)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse<Relations>(value.ToString());
        }
    }
}
