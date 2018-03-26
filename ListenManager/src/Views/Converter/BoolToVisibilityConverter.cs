using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ListenManager.Views.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        private static object GetVisibility(object value)
        {
            if (!(value is bool))
                return Visibility.Collapsed;
            var objValue = (bool)value;
            return objValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static object GetBool(object value)
        {
            if (!(value is Visibility))
            {
                return true;
            }

            var objValue = (Visibility)value;
            switch (objValue)
            {
                case Visibility.Visible:
                    return true;
                case Visibility.Hidden:
                    return false;
                case Visibility.Collapsed:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetBool(value);
        }
    }
}