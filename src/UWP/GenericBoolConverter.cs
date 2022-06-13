// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;
#if MAUI
using Microsoft.UI.Xaml.Data;

#else
using Windows.UI.Xaml.Data;

#endif
namespace APES.UI.XF.UWP
{
    internal class GenericBoolConverter<T> : IValueConverter
    {
        public GenericBoolConverter(T @true, T @false)
        {
            True = @true ?? throw new ArgumentNullException(nameof(@true));
            False = @false ?? throw new ArgumentNullException(nameof(@false));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public T True { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public T False { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? boolean = value as bool?;
            return (boolean ?? false ? True : False) !;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
    }
}
