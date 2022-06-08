#if MAUI && (__ANDROID__ || __IOS__ || __MACCATALYST__)// || __WINDOWS__)
using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Handlers;
namespace APES.UI.XF;

    [Preserve(AllMembers = true)]
    sealed partial class ContextMenuContainerHandler: ContentViewHandler
    {
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
    }
#endif
