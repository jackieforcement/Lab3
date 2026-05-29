using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfLab3.Helpers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }
        public Visibility HiddenValue { get; set; }

        public BoolToVisibilityConverter()
        {
            Invert = false;
            HiddenValue = Visibility.Collapsed;
        }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool flag;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is string)
            {
                string text = (string)value;
                flag = !string.IsNullOrWhiteSpace(text);
            }
            else if (value == null)
            {
                flag = false;
            }
            else
            {
                flag = true;
            }

            if (Invert)
            {
                flag = !flag;
            }

            if (flag)
            {
                return Visibility.Visible;
            }
            return HiddenValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
