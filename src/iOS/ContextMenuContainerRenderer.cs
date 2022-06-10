using System;
using UIKit;
#if MAUI 
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using APES.UI.XF.iOS;

namespace APES.UI.XF;

sealed partial class ContextMenuContainerRenderer : ContentViewHandler
{
UIView Control => PlatformView;
UITraitCollection TraitCollection => UITraitCollection.CurrentTraitCollection;

//TODO: not sure if this is really needed, will test when debug is available for MAUI 
bool wasSetOnce = false;
public override void SetVirtualView(IView view)
{
    if (wasSetOnce)
    {
        var old = VirtualView as ContextMenuContainer;
        old.BindingContextChanged -= Element_BindingContextChanged;
        old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;
    }

    base.SetVirtualView(view);

    if (VirtualView is ContextMenuContainer newElement)
    {
        newElement.BindingContextChanged += Element_BindingContextChanged;
        newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;

        //PlatformView.AddSubview(newElement.Content.ToContainerView(MauiContext));
        //SetContent();
        //PlatformView.View = newElement;
        RefillMenuItems();
        wasSetOnce = true;
    }
}


void RefillMenuItems()
{
    if (VirtualView is ContextMenuContainer container)
    {
        constructInteraction(container);
    }
}

void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
{
    RefillMenuItems();
}

void Element_BindingContextChanged(object sender, EventArgs e)
{
    RefillMenuItems();
}
#else
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
using APES.UI.XF;
using APES.UI.XF.iOS;

[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
namespace APES.UI.XF.iOS
{
    [Preserve(AllMembers = true)
    class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                //do something with old element
            }
            if (e.NewElement == null || e.NewElement.Content == null)
            {
                return;
            }
            var childRenderer = Element.Content.GetRenderer() ?? Platform.CreateRenderer(Element.Content);
            SetNativeControl(childRenderer.NativeView);
            constructInteraction(e.NewElement.MenuItems);

        }
        ContextMenuContainer VirtualView => Element;
    
        void RefillMenuItems() => constructInteraction(((ContextMenuContainer)VirtualView).MenuItems);
        void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefillMenuItems();
        }

        void Element_BindingContextChanged(object sender, EventArgs e)
        {
            RefillMenuItems();
        }
#endif

    ContextMenuDelegate? contextMenuDelegate;
    UIContextMenuInteraction? contextMenu;

    void deconstructIntercation()
        {
            if (Control != null && contextMenu != null)
            {
                Control.RemoveInteraction(contextMenu);
                //contextMenuDelegate?.Dispose();
                //contextMenu?.Dispose();
            }
        }
        void constructInteraction(ContextMenuContainer container)
        {
            deconstructIntercation();
            if (container.MenuItems?.Count > 0)
            {
                contextMenuDelegate = new ContextMenuDelegate(container.MenuItems, () => TraitCollection.UserInterfaceStyle);
                contextMenu = new UIContextMenuInteraction(contextMenuDelegate);
                Control.AddInteraction(contextMenu);
            }
        }
    }
#if !MAUI
}
#endif
