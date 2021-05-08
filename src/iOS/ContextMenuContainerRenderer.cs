using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using APES.UI.XF;
using APES.UI.XF.iOS;
[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
namespace APES.UI.XF.iOS
{
    class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, UIView>
    {
        ContextMenuDelegate? contextMenuDelegate;
        UIContextMenuInteraction? contextMenu;
        UIView? nativeView;
        protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {

            }
            if (e.NewElement == null || e.NewElement.Content == null)
            {
                return;
            }
            e.NewElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
            var childRenderer = Platform.CreateRenderer(Element.Content);
            nativeView = childRenderer.NativeView;
            constructInteraction();
            SetNativeControl(nativeView);
        }
        private void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is ContextMenuItems menuItems)
            {
                if (contextMenu == null && menuItems.Count > 0)
                {
                    constructInteraction();
                }
                else if (menuItems.Count == 0 && contextMenu != null)
                {
                    deconstructIntercation();
                }
            }
        }
        void deconstructIntercation()
        {
            if (contextMenu != null)
            {
                nativeView?.RemoveInteraction(contextMenu);
            }
        }
        void constructInteraction()
        {
            if (nativeView == null)
                return;
            deconstructIntercation();
            if (Element?.MenuItems.Count > 0)
            {
                contextMenuDelegate = new ContextMenuDelegate(Element.MenuItems, () => TraitCollection.UserInterfaceStyle);
                contextMenu = new UIContextMenuInteraction(contextMenuDelegate);
                nativeView.AddInteraction(contextMenu);
            }
        }
    }
}
