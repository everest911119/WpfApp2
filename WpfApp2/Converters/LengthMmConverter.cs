using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp2.Converters
{
    public sealed class LengthMmConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int length ? $"{length} mm" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string text)
            {
                return Binding.DoNothing;
            }

            text = text.Replace("mm", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

            return int.TryParse(text, NumberStyles.Integer, culture, out var length)
                ? length
                : Binding.DoNothing;
        }
    }
}