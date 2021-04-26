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
            var childRenderer = Platform.CreateRenderer(Element.Content);
            var nativeView = childRenderer.NativeView;

            if (Element?.MenuItems.Count > 0)
            {
                contextMenuDelegate = new ContextMenuDelegate(Element.MenuItems, ()=> TraitCollection.UserInterfaceStyle);
                contextMenu = new UIContextMenuInteraction(contextMenuDelegate);
                nativeView.AddInteraction(contextMenu);
            }
            SetNativeControl(nativeView);
        }
    }
}
