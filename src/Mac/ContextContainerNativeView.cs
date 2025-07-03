// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;
using AppKit;
using CoreGraphics;
using Foundation;
using Xamarin.Forms.Platform.MacOS;

namespace APES.MAUI.Mac
{
    public sealed class ContextContainerNativeView : NSView
    {
        private readonly ContextMenuItems? _menuItems;
        private NSMenu? _contextMenu;

        public ContextContainerNativeView(IVisualElementRenderer childRenderer, ContextMenuItems? contextMenuItems)
        {
            if (childRenderer == null)
            {
                throw new ArgumentNullException(nameof(childRenderer));
            }

            _menuItems = contextMenuItems;
            if (_menuItems != null)
            {
                _menuItems.CollectionChanged += MenuItems_CollectionChanged;
            }

            AddSubview(childRenderer.NativeView);
        }

        public override void RightMouseDown(NSEvent theEvent)
        {
            HandleContextActions(theEvent);

            base.RightMouseDown(theEvent);
        }

        private void MenuItems_CollectionChanged(
            object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => RefillMenuItems();

        private NSMenu? GetContextMenu()
        {
            if (_contextMenu != null && _menuItems != null)
            {
                if (_menuItems.Count != _contextMenu.Count)
                {
                    RefillMenuItems();
                }
                else
                {
                    for (int i = 0; i < _contextMenu.Count; i++)
                    {
                        var nativeItem = _contextMenu.ItemWithTag(i);
                        if (!_menuItems[i].Text.Equals(nativeItem?.AttributedTitle.Value))
                        {
                            RefillMenuItems();
                            break;
                        }
                    }
                }
            }

            return _contextMenu;
        }

        private void HandleContextActions(NSEvent theEvent)
        {
            if (_menuItems == null)
            {
                return;
            }

            int count = _menuItems.Count;
            if (count == 0)
            {
                return;
            }

            if (GetContextMenu() == null)
            {
                ConstructNativeMenu();
                FillMenuItems();
            }

            NSMenu.PopUpContextMenu(_contextMenu!, theEvent, this);
        }

        private void ConstructNativeMenu() => _contextMenu = new NSMenu();

        private void FillMenuItems()
        {
            if (_menuItems?.Count > 0)
            {
                for (int i = 0; i < _menuItems.Count; i++)
                {
                    _contextMenu!.AddItem(ToNSMenuItem(i, _menuItems[i]));
                }

                _contextMenu!.Update();
            }
        }

        private void RefillMenuItems()
        {
            if (_contextMenu == null)
            {
                return;
            }

            _contextMenu.CancelTracking();
            _contextMenu.RemoveAllItems();
            FillMenuItems();
        }

#pragma warning disable SA1202

        // ReSharper disable once InconsistentNaming
        public NSMenuItem ToNSMenuItem(int i, ContextMenuItem menuItem)
#pragma warning restore SA1202
        {
            NSMenuItem nsMenuItem = new NSMenuItem();
            nsMenuItem.AttributedTitle = new NSAttributedString(
                menuItem.Text,
                foregroundColor: menuItem.IsDestructive ? NSColor.Red : null);
            nsMenuItem.Tag = i;
            nsMenuItem.Enabled = menuItem.IsEnabled;
            nsMenuItem.Activated += NsMenuItem_Activated;
            nsMenuItem.ValidateMenuItem = (t) => t.Enabled;
            var nativeIcon = menuItem.Icon?.ToNative();
            if (nativeIcon != null)
            {
                var elementColor = menuItem.IsDestructive ? NSColor.Red :
                    NSAppearance.CurrentAppearance.Name == NSAppearance.NameDarkAqua ? NSColor.White : NSColor.Black;
                nsMenuItem.Image = ImageHandler.ImageTintedWithColor(nativeIcon, elementColor, new CGSize(25, 25));
            }

            return nsMenuItem;
        }

        private void NsMenuItem_Activated(object sender, EventArgs e)
        {
            var nsMenuItem = sender as NSMenuItem;
            if (nsMenuItem == null)
            {
                Logger.Error("Couldn't cast sender to NSMenuItem");
                return;
            }

            // that seems to have no effect
            if (nsMenuItem.Enabled)
            {
                _menuItems?[(int)nsMenuItem.Tag].OnItemTapped();
            }
        }
    }
}
