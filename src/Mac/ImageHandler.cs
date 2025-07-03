// MIT License
// Copyright (c) 2021 Pavel Anpin

using System.IO;
using AppKit;
using CoreGraphics;
using Xamarin.Forms;

namespace APES.MAUI.Mac
{
    public static class ImageHandler
    {
        public static NSImage? ToNative(this FileImageSource? source)
        {
            NSImage? image = null;
            string? file = source?.File;
            if (!string.IsNullOrWhiteSpace(file))
            {
                image = File.Exists(file) ? new NSImage(file!) : NSImage.ImageNamed(file!);
            }

            return image;
        }

        public static NSImage ImageTintedWithColor(NSImage sourceImage, NSColor tintColor, CGSize? size = null) =>
            NSImage.ImageWithSize(size ?? sourceImage.Size, false, rect =>
            {
                // Draw the original source image
                sourceImage.Draw(rect, CGRect.Empty, NSCompositingOperation.SourceOver, 1f);

                // Apply tint
                tintColor.Set();
                NSGraphics.RectFill(rect, NSCompositingOperation.SourceAtop);

                return true;
            });
    }
}
