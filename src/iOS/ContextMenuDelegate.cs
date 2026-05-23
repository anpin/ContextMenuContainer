// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using CoreGraphics;

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
// ReSharper disable once CheckNamespace
#pragma warning disable SA1300
namespace APES.MAUI.iOS;
#pragma warning restore SA1300
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

    private IEnumerable<UIMenuElement> ToNativeActions(IEnumerable<BaseContextMenuItem> sharedDefinitions)
    {
        var iconColor = _getCurrentTheme() == UIUserInterfaceStyle.Dark ? UIColor.White : UIColor.Black;
        var items = sharedDefinitions.ToList();
        var groups = new List<List<ContextMenuItem>>();
        var currentGroup = new List<ContextMenuItem>();

        // Group items by separators
        foreach (var item in items)
        {
            switch (item)
            {
                case ContextMenuSeparator:
                    if (currentGroup.Count > 0)
                    {
                        groups.Add(currentGroup);
                        currentGroup = new List<ContextMenuItem>();
                    }

                    break;

                case ContextMenuItem contextItem:
                    currentGroup.Add(contextItem);
                    break;
            }
        }

        // Add the last group if it has items
        if (currentGroup.Count > 0)
        {
            groups.Add(currentGroup);
        }

        // Convert each group to a UIMenu with DisplayInline option
        foreach (var group in groups)
        {
            var groupActions = new List<UIMenuElement>();

            foreach (var contextItem in group)
            {
                if (!string.IsNullOrEmpty(contextItem.Text))
                {
                    UIImage? nativeImage = null;
                    if (contextItem.Icon is not null && !string.IsNullOrWhiteSpace(contextItem.Icon.File))
                    {
                        nativeImage = UIImage.FromBundle(contextItem.Icon.File);
                        nativeImage = nativeImage?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                        nativeImage?.ApplyTintColor(contextItem.IsDestructive ? UIColor.Red : iconColor);
                    }

                    var nativeItem = UIAction.Create(contextItem.Text, nativeImage, contextItem.Text, ActionDelegate);
                    if (!contextItem.IsEnabled)
                    {
                        nativeItem.Attributes |= UIMenuElementAttributes.Disabled;
                    }

                    if (contextItem.IsDestructive)
                    {
                        nativeItem.Attributes |= UIMenuElementAttributes.Destructive;
                    }

                    groupActions.Add(nativeItem);
                }
                else
                {
                    Logger.Error("ContextMenuItem text should not be empty!");
                }
            }

            if (groupActions.Count > 0)
            {
                yield return UIMenu.Create(string.Empty, null, UIMenuIdentifier.None, UIMenuOptions.DisplayInline,
                    groupActions.ToArray());
            }
        }
    }

    private void ActionDelegate(UIAction action) => _menuItems[action.Identifier].OnItemTapped();

    private UIMenu ConstructMenuFromItems(UIMenuElement[] suggestedActions)
    {
        _nativeMenu = _nativeMenu is null ?
            UIMenu.Create(ToNativeActions(_menuItems).ToArray()) :
            _nativeMenu.GetMenuByReplacingChildren(ToNativeActions(_menuItems).ToArray());

        return _nativeMenu;
    }

    private UIViewController? PreviewDelegate() => _preview?.Invoke();
}
