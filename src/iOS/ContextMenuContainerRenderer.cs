using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
#if MAUI 
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ContentView = Microsoft.Maui.Platform.ContentView;
#else
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
#endif 
using APES.UI.XF;
using APES.UI.XF.iOS;
#if !MAUI
[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
#endif
namespace APES.UI.XF.iOS
{
    [Preserve(AllMembers = true)]
    #if MAUI
    class ContextMenuContainerRenderer : ContentViewHandler  //ViewHandler<ContextMenuContainer, ContentView>
#else
    class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, UIView>
#endif
    {
        ContextMenuDelegate? contextMenuDelegate;
        UIContextMenuInteraction? contextMenu;
        #if MAUI 
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

        UIView Control => PlatformView;
        UITraitCollection TraitCollection => UITraitCollection.CurrentTraitCollection;
#else
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
#endif
        void deconstructIntercation()
        {
            if (Control != null && contextMenu != null)
            {
                Control.RemoveInteraction(contextMenu);
                //contextMenuDelegate?.Dispose();
                //contextMenu?.Dispose();
            }
        }
        void constructInteraction(ContextMenuItems menuItems)
        {
            deconstructIntercation();
            if (menuItems?.Count > 0)
            {
                contextMenuDelegate = new ContextMenuDelegate(menuItems, () => TraitCollection.UserInterfaceStyle);
                contextMenu = new UIContextMenuInteraction(contextMenuDelegate);
                Control.AddInteraction(contextMenu);
            }
        }

        void RefillMenuItems() => constructInteraction(((ContextMenuContainer)VirtualView).MenuItems);
        private void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefillMenuItems();
        }

        private void Element_BindingContextChanged(object sender, EventArgs e)
        {
            RefillMenuItems();
        }
    }
}
