using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfLab3.Helpers;

public class BoolToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }
    public Visibility HiddenValue { get; set; } = Visibility.Collapsed;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var flag = value switch
        {
            bool b => b,
            string s => !string.IsNullOrWhiteSpace(s),
            null => false,
            _ => true,
        };
        if (Invert) flag = !flag;
        return flag ? Visibility.Visible : HiddenValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}
