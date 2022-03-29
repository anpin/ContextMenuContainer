using System;
using System.Collections.Generic;
#if MAUI
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
#if __ANDROID__
using APES.UI.XF.Droid;
#elif __IOS__ || __MACCATALYST__
using APES.UI.XF.iOS;
#elif NET6_0_WINDOWS10_0_17763_0
using APES.UI.XF.UWP;
#endif
#endif

namespace APES.UI.XF
{
    public static class ContextMenuContainerExtensions
    {
        public static bool HasMenuOptions(this ContextMenuContainer container) => container.MenuItems.Count > 0;
#if MAUI && (__ANDROID__ || __IOS__ || __MACCATALYST__ || NET6_0_WINDOWS10_0_17763_0)
        public static MauiAppBuilder ConfigureContextMenuContainer(this MauiAppBuilder mauiAppBuilder)
        {
            
            return mauiAppBuilder.ConfigureMauiHandlers(handlers =>
             {

#if __ANDROID__
                handlers.AddCompatibilityRenderer<ContextMenuContainer, ContextMenuContainerRenderer>();
                
#elif __IOS__ || __MACCATALYST__
                 //handlers.AddCompatibilityRenderer<ContextMenuContainer, ContextMenuContainerRenderer>();
                 handlers.AddHandler<ContextMenuContainer, ContextMenuContainerRenderer>();
#elif NET6_0_WINDOWS10_0_17763_0
                 handlers.AddCompatibilityRenderer<ContextMenuContainer, ContextMenuContainerRenderer>();
#endif
             });
        }
#endif
    }
}