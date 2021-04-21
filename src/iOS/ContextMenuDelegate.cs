using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Foundation;
using CoreGraphics;

namespace APES.UI.XF.iOS
{
    class ContextMenuDelegate : UIContextMenuInteractionDelegate
    {
        readonly INSCopying? identifier;
        readonly Func<UIViewController>? preview;
        readonly ContextMenuItems menuItems;
        public ContextMenuDelegate(ContextMenuItems items, INSCopying? identifier = null, Func<UIViewController>? preview = null)
        {
            menuItems = items ?? throw new ArgumentNullException(nameof(items));
            this.identifier = identifier;
            this.preview = preview;
        }
        IEnumerable<UIAction> ToNativeActions(IEnumerable<ContextMenuItem> sharedDefenitions)
        {
            foreach (var item in sharedDefenitions)
            {
                var nativeItem = UIAction.Create(item.Text, item.Icon != null ? new UIImage(item.Icon.File) : null, item.Text, ActionDelegate);
                if (!item.IsEnabled)
                    nativeItem.Attributes |= UIMenuElementAttributes.Disabled;
                if (item.IsDestructive)
                    nativeItem.Attributes |= UIMenuElementAttributes.Destructive;
                yield return nativeItem;
            }
        }
        void ActionDelegate(UIAction action)
        {
            var item = menuItems.FirstOrDefault(item => item.Text == action.Identifier);
            if (item != null && item.Command != null && item.Command.CanExecute(item.CommandParameter))
                item.Command.Execute(item.CommandParameter);
        }
        UIMenu ContructMenuFromItems(UIMenuElement[] suggestedActions)
        {
            return UIMenu.Create(ToNativeActions(menuItems).ToArray());
        }

        UIViewController? PreviewDelegate()
        {
            return preview?.Invoke();
        }

        public override UIContextMenuConfiguration GetConfigurationForMenu(UIContextMenuInteraction interaction, CGPoint location)
        {
            return UIContextMenuConfiguration.Create(identifier, preview != null ? PreviewDelegate : null, ContructMenuFromItems);
        }
    }
}
