// MIT License
// Copyright (c) 2021 Pavel Anpin

using UIKit;
#if MAUI
using System;
using System.Collections.Specialized;
using APES.UI.XF.iOS;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace APES.UI.XF
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
#else
using APES.UI.XF;
using APES.UI.XF.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]

#pragma warning disable SA1300
namespace APES.UI.XF.iOS
#pragma warning restore SA1300
{
    [Preserve(AllMembers = true)]
    internal class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                // do something with old element
            }

            if (e.NewElement == null || e.NewElement.Content == null)
            {
                return;
            }

            var childRenderer = Element.Content.GetRenderer() ?? Platform.CreateRenderer(Element.Content);
            SetNativeControl(childRenderer.NativeView);
            ConstructInteraction(e.NewElement);
        }
#endif

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
