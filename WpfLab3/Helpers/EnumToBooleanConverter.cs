using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfLab3.Helpers
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            if (parameter == null)
            {
                return false;
            }
            string? parameterText = parameter.ToString();
            if (parameterText == null)
            {
                return false;
            }
            object parsed = Enum.Parse(value.GetType(), parameterText);
            return value.Equals(parsed);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return Binding.DoNothing;
            }
            bool flag = (bool)value;
            if (!flag)
            {
                return Binding.DoNothing;
            }
            if (parameter == null)
            {
                return Binding.DoNothing;
            }
            string? parameterText = parameter.ToString();
            if (parameterText == null)
            {
                return Binding.DoNothing;
            }
            return Enum.Parse(targetType, parameterText);
        }
    }
}
