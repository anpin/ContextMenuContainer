// MIT License
// Copyright (c) 2021 Pavel Anpin

using UIKit;
using System;
using System.Collections.Specialized;
using APES.MAUI.iOS;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace APES.MAUI
{
    internal sealed class ContextMenuContainerRenderer : ContentViewHandler
    {
        private bool _wasSetOnce;

        private UIView Control => PlatformView;

        private UITraitCollection TraitCollection => UITraitCollection.CurrentTraitCollection;

        public override void SetVirtualView(IView view)
        {
            if (_wasSetOnce)
            {
                if (VirtualView is ContextMenuContainer old)
                {
                    old.BindingContextChanged -= Element_BindingContextChanged;
                    if (old.MenuItems != null)
                    {
                        old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;
                    }
                }
            }

            base.SetVirtualView(view);

            if (VirtualView is ContextMenuContainer newElement)
            {
                newElement.BindingContextChanged += Element_BindingContextChanged;
                if (newElement.MenuItems != null)
                {
                    newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
                }

                RefillMenuItems();
                _wasSetOnce = true;
            }
        }

        private void RefillMenuItems()
        {
            if (VirtualView is ContextMenuContainer container)
            {
                ConstructInteraction(container);
            }
        }

        private void MenuItems_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e) => RefillMenuItems();

        private void Element_BindingContextChanged(object? sender, EventArgs e) => RefillMenuItems();

#pragma warning disable SA1201
        private ContextMenuDelegate? _contextMenuDelegate;
#pragma warning restore SA1201
        private UIContextMenuInteraction? _contextMenu;

        private void DeconstructInteraction()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Control != null && _contextMenu != null)
            {
                Control.RemoveInteraction(_contextMenu);
            }
        }

        private void ConstructInteraction(ContextMenuContainer container)
        {
            DeconstructInteraction();
            if (container.MenuItems?.Count > 0)
            {
                _contextMenuDelegate =
                    new ContextMenuDelegate(container.MenuItems, () => TraitCollection.UserInterfaceStyle);
                _contextMenu = new UIContextMenuInteraction(_contextMenuDelegate);
                Control.AddInteraction(_contextMenu);
            }
        }
    }
}
