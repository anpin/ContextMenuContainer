// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;
#if MAUI
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IValueConverter = Microsoft.UI.Xaml.Data.IValueConverter;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using FileImageSource = Xamarin.Forms.FileImageSource;
using IOPath = System.IO.Path;

#endif

// ReSharper disable once CheckNamespace
namespace APES.UI.XF.UWP
{
    internal class FileImageSourceToBitmapIconSourceConverter : IValueConverter
    {
        private readonly Uri _baseUri = new ("ms-appx:///");

        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            IconElement? result = null;
            if (value is not FileImageSource source)
            {
                return result;
            }

            if (!string.IsNullOrWhiteSpace(source.File))
            {
                result = new BitmapIcon()
                {
                    UriSource = new Uri(_baseUri, source.File),
                };
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
    }
}
