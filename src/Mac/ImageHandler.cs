using System.IO;
using AppKit;
using CoreGraphics;
using Xamarin.Forms;
namespace APES.UI.XF.Mac
{
    public static class ImageHandler
    {
        public static NSImage? ToNative(this FileImageSource source)
        {
            NSImage? image = null;
            var file = source?.File;
            if (!string.IsNullOrWhiteSpace(file))
                image = File.Exists(file) ? new NSImage(file) : NSImage.ImageNamed(file);
            return image;
        }
        public static NSImage ImageTintedWithColor(NSImage sourceImage, NSColor tintColor, CGSize? size = null)
        {
            return NSImage.ImageWithSize(size ?? sourceImage.Size, false, rect =>
            {
                // Draw the original source image
                sourceImage.DrawInRect(rect, CGRect.Empty, NSCompositingOperation.SourceOver, 1f);

                // Apply tint
                tintColor.Set();
                NSGraphics.RectFill(rect, NSCompositingOperation.SourceAtop);

                return true;
            });
        }
    }
}
