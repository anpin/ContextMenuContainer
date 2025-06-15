// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;
using System.Collections.Specialized;
using APES.MAUI.UWP;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using MenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using Setter = Microsoft.UI.Xaml.Setter;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;
using Style = Microsoft.UI.Xaml.Style;
using WBinding = Microsoft.UI.Xaml.Data.Binding;
using WColors = Microsoft.UI.Colors;
using WControl = Microsoft.UI.Xaml.Controls.Control;

namespace APES.MAUI
{
    [Preserve(AllMembers = true)]
    internal sealed class ContextMenuContainerRenderer : ContentViewHandler
    {
        private ContextMenuContainer Element => (ContextMenuContainer)VirtualView;

        protected override ContentPanel CreatePlatformView()
        {
            var result = base.CreatePlatformView();
            result.PointerReleased += PlatformViewPointerReleased;
            return result;
        }

        // ReSharper disable once InconsistentNaming
#pragma warning disable SA1300
#pragma warning disable SA1201
        private FrameworkElement _platformView => PlatformView;
#pragma warning restore SA1201
#pragma warning restore SA1300

        private void PlatformViewPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(_platformView);
            if (point.Properties.PointerUpdateKind != PointerUpdateKind.RightButtonReleased)
            {
                return;
            }

            try
            {
                if (Element.HasMenuOptions())
                {
                    OpenContextMenu();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private MenuFlyout? GetContextMenu()
        {
            if (FlyoutBase.GetAttachedFlyout(_platformView) is MenuFlyout flyout)
            {
                var actions = Element.MenuItems;
                if (flyout.Items?.Count != actions?.Count)
                {
                    return null;
                }

                for (int i = 0; i < flyout.Items?.Count; i++)
                {
                    if (flyout.Items[i].DataContext != actions?[i])
                    {
                        return null;
                    }
                }

                return flyout;
            }

            return null;
        }

        private void OpenContextMenu()
        {
            if (GetContextMenu() == null)
            {
                var flyout = new MenuFlyout();
                SetupMenuItems(flyout);

                if (Element.MenuItems != null)
                {
                    Element.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
                }

                FlyoutBase.SetAttachedFlyout(_platformView, flyout);
            }

            FlyoutBase.ShowAttachedFlyout(_platformView);
        }

        private void MenuItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var menu = GetContextMenu();
            if (menu != null)
            {
                if (menu.Items != null)
                {
                    menu.Items.Clear();
                }

                SetupMenuItems(menu);
            }
        }

        private void SetupMenuItems(MenuFlyout menu)
        {
            if (Element.MenuItems != null)
            {
                foreach (var item in Element.MenuItems)
                {
                    AddMenuItem(menu, item);
                }
            }
        }

        private void AddMenuItem(MenuFlyout contextMenu, ContextMenuItem item)
        {
            var nativeItem = new MenuFlyoutItem();
            nativeItem.SetBinding(
                MenuFlyoutItem.TextProperty,
                new WBinding() { Path = new PropertyPath(nameof(ContextMenuItem.Text)) });

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (ImageConverter != null)
            {
                nativeItem.SetBinding(
                    MenuFlyoutItem.IconProperty,
                    new WBinding() { Path = new PropertyPath(nameof(ContextMenuItem.Icon)), Converter = ImageConverter });
            }

            nativeItem.SetBinding(
                FrameworkElement.StyleProperty,
                new WBinding()
                {
                    Path = new PropertyPath(nameof(ContextMenuItem.IsDestructive)),
                    Converter = BoolToStyleConverter,
                });
            nativeItem.SetBinding(
                WControl.IsEnabledProperty,
                new WBinding() { Path = new PropertyPath(nameof(ContextMenuItem.IsEnabled)) });
            nativeItem.Click += NativeItem_Click;
            nativeItem.DataContext = item;
            if (contextMenu.Items != null)
            {
                contextMenu.Items.Add(nativeItem);
            }
        }

        private void NativeItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            if (item == null)
            {
                Logger.Error("Couldn't cast to MenuFlyoutItem");
                return;
            }

            if (item.DataContext is not ContextMenuItem context)
            {
                Logger.Error("Couldn't cast MenuFlyoutItem.DataContext to ContextMenuItem");
                return;
            }

            context.OnItemTapped();
        }

#pragma warning disable SA1201
        private static Style DestructiveStyle { get; } = new Style()
#pragma warning restore SA1201
        {
            TargetType = typeof(MenuFlyoutItem),
            Setters =
            {
                new Setter(
                    WControl.ForegroundProperty,
                    new SolidColorBrush(WColors.Red)),
            },
        };

        private static Style NonDestructiveStyle { get; } = new Style()
        {
            TargetType = typeof(MenuFlyoutItem),
            Setters =
            {
                // new Setter(MenuFlyoutItem.ForegroundProperty, new SolidColorBrush(WColors.Red)),
            },
        };

        private static FileImageSourceToBitmapIconSourceConverter ImageConverter { get; } =
            new FileImageSourceToBitmapIconSourceConverter();

        private static GenericBoolConverter<Style> BoolToStyleConverter { get; } =
            new (DestructiveStyle, NonDestructiveStyle);
    }
}
