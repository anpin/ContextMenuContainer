using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;

namespace APES.UI.XF.iOS
{
    class ContextMenuDelegate : UIContextMenuInteractionDelegate
    {
        readonly INSCopying? Identifier;
        readonly Func<UIViewController>? Preview;
        readonly ContextMenuItems MenuItems;
        readonly Func<UIUserInterfaceStyle> GetCurrentTheme;
        UIMenu? nativeMenu;
        public ContextMenuDelegate(ContextMenuItems items, Func<UIUserInterfaceStyle> getCurrentTheme, INSCopying? identifier = null, Func<UIViewController>? preview = null)
        {
            MenuItems = items ?? throw new ArgumentNullException(nameof(items));
            this.Identifier = identifier;
            this.Preview = preview;
            GetCurrentTheme = getCurrentTheme;
        }

        IEnumerable<UIAction> ToNativeActions(IEnumerable<ContextMenuItem> sharedDefenitions)
        {
            var iconColor = GetCurrentTheme() == UIUserInterfaceStyle.Dark ? UIColor.White : UIColor.Black;
            foreach (var item in sharedDefenitions)
            {
                if (!string.IsNullOrEmpty(item.Text))
                {
                    UIImage? nativeImage = null;
                    if (item.Icon != null && !string.IsNullOrWhiteSpace(item.Icon.File))
                    {
                        nativeImage = new UIImage(item.Icon.File);
                        nativeImage = nativeImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                        nativeImage.ApplyTintColor(item.IsDestructive ? UIColor.Red : iconColor);
                    }
                    var nativeItem = UIAction.Create(item.Text, nativeImage, item.Text, ActionDelegate);
                    if (!item.IsEnabled)
                        nativeItem.Attributes |= UIMenuElementAttributes.Disabled;
                    if (item.IsDestructive)
                        nativeItem.Attributes |= UIMenuElementAttributes.Destructive;
                    yield return nativeItem;
                }
                else
                {
                    Logger.Error("ContextMenuItem text should not be empty!");
                }
            }
        }
        void ActionDelegate(UIAction action)
        {
            MenuItems[action.Identifier].InvokeCommand();            
        }
        UIMenu ContructMenuFromItems(UIMenuElement[] suggestedActions)
        {
            if (nativeMenu == null)
                nativeMenu = UIMenu.Create(ToNativeActions(MenuItems).ToArray());
            else
                nativeMenu = nativeMenu.GetMenuByReplacingChildren(ToNativeActions(MenuItems).ToArray());
            return nativeMenu;
        }

        UIViewController? PreviewDelegate()
        {
            return Preview?.Invoke();
        }

        public override UIContextMenuConfiguration GetConfigurationForMenu(UIContextMenuInteraction interaction, CGPoint location)
        {
            return UIContextMenuConfiguration.Create(Identifier, Preview != null ? PreviewDelegate : null, ContructMenuFromItems);
        }
    }
}
