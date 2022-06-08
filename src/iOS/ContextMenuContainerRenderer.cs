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
#if MAUI
namespace APES.UI.XF

#else
[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
namespace APES.UI.XF.iOS
#endif
{

#if MAUI
    sealed partial class ContextMenuContainerHandler : ContentViewHandler  //ViewHandler<ContextMenuContainer, ContentView>
#else
    [Preserve(AllMembers = true)
    class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, UIView>
#endif
    {
        ContextMenuDelegate? contextMenuDelegate;
        UIContextMenuInteraction? contextMenu;
        #if MAUI
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
#if !MAUI
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
    }
}
