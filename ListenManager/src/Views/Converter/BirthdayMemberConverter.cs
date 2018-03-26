using System;
using System.Globalization;
using System.Windows.Data;

namespace ListenManager.Views.Converter
{
    internal class BirthdayMemberConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is DateTime && values[1] is string && values[2] is string)
            {
                return string.Format("{0:dd.MM.}, {1} {2}", values);
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
