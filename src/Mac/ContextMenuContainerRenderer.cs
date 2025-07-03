// MIT License
// Copyright (c) 2021 Pavel Anpin

using APES.MAUI;
using APES.MAUI.Mac;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]

namespace APES.MAUI.Mac
{
    [Preserve(AllMembers = true)]
    internal class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, ContextContainerNativeView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null || e.NewElement.Content == null)
            {
                return;
            }

            var childRenderer = Platform.CreateRenderer(Element.Content);
            var nativeContainer = new ContextContainerNativeView(childRenderer, Element.MenuItems);
            SetNativeControl(nativeContainer);
        }
    }
}
