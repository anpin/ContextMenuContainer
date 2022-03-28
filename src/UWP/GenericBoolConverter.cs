using System;
#if MAUI
using Microsoft.UI.Xaml.Data;
#else
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
#endif
namespace APES.UI.XF.UWP
{
    class GenericBoolConverter<T> : IValueConverter
    {
        public T True { get; set; }
        public T False { get; set; }
        public GenericBoolConverter(T True, T False)
        {

            this.True = True ?? throw new ArgumentNullException(nameof(True));
            this.False = False ?? throw new ArgumentNullException(nameof(False));
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? boolean = value as bool?;
            return (boolean ?? false) ? True : False;

        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

    }
}
