// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
#if MAUI
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
#endif
using CoreGraphics;

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
// ReSharper disable once CheckNamespace
#pragma warning disable SA1300
namespace APES.UI.XF.iOS
#pragma warning restore SA1300
{
    internal class ContextMenuDelegate : UIContextMenuInteractionDelegate
    {
        private readonly INSCopying? _identifier;
        private readonly Func<UIViewController>? _preview;
        private readonly ContextMenuItems _menuItems;
        private readonly Func<UIUserInterfaceStyle> _getCurrentTheme;
        private UIMenu? _nativeMenu;

        public ContextMenuDelegate(ContextMenuItems items, Func<UIUserInterfaceStyle> getCurrentTheme, INSCopying? identifier = null, Func<UIViewController>? preview = null)
        {
            _menuItems = items ?? throw new ArgumentNullException(nameof(items));
            _identifier = identifier;
            _preview = preview;
            _getCurrentTheme = getCurrentTheme;
        }

        public override UIContextMenuConfiguration GetConfigurationForMenu(UIContextMenuInteraction interaction, CGPoint location)
            => UIContextMenuConfiguration.Create(_identifier, _preview != null ? PreviewDelegate! : null, ConstructMenuFromItems);

        private IEnumerable<UIMenuElement> ToNativeActions(IEnumerable<ContextMenuItem> sharedDefinitions)
        {
            var iconColor = _getCurrentTheme() == UIUserInterfaceStyle.Dark ? UIColor.White : UIColor.Black;
            foreach (var item in sharedDefinitions)
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
                    {
                        nativeItem.Attributes |= UIMenuElementAttributes.Disabled;
                    }

                    if (item.IsDestructive)
                    {
                        nativeItem.Attributes |= UIMenuElementAttributes.Destructive;
                    }

                    yield return nativeItem;
                }
                else
                {
                    Logger.Error("ContextMenuItem text should not be empty!");
                }
            }
        }

        private void ActionDelegate(UIAction action) => _menuItems[action.Identifier].OnItemTapped();

        private UIMenu ConstructMenuFromItems(UIMenuElement[] suggestedActions)
        {
            _nativeMenu = _nativeMenu == null ?
                UIMenu.Create(ToNativeActions(_menuItems).ToArray()) :
                _nativeMenu.GetMenuByReplacingChildren(ToNativeActions(_menuItems).ToArray());
            return _nativeMenu;
        }

        private UIViewController? PreviewDelegate() => _preview?.Invoke();
    }
}
