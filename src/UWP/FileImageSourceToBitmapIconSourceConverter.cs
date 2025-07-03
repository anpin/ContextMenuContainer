// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;

using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IValueConverter = Microsoft.UI.Xaml.Data.IValueConverter;

// ReSharper disable once CheckNamespace
namespace APES.MAUI.UWP;
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
