using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
using APES.UI.XF;
using APES.UI.XF.iOS;
[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
namespace APES.UI.XF.iOS
{
    [Preserve(AllMembers = true)]
    class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, UIView>
    {
        ContextMenuDelegate? contextMenuDelegate;
        UIContextMenuInteraction? contextMenu;
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
            var childRenderer = Platform.CreateRenderer(Element.Content);
            SetNativeControl(childRenderer.NativeView);
            constructInteraction(e.NewElement.MenuItems);

        }
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
    }
}
