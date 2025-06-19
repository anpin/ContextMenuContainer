using System.Globalization;

namespace APES.MAUI.Sample;

public class BoolToStringConverter : IValueConverter
{
    public string True { get; set; }
    public string False { get; set; }

    public BoolToStringConverter() : this(string.Empty, string.Empty)
    {
    }

    public BoolToStringConverter(string True, string False)
    {
        this.True = True;
        this.False = False;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool) return (bool)value ? True : False;
        else return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string) return (string)value == True ? true : false;
        else return false;
    }
}
