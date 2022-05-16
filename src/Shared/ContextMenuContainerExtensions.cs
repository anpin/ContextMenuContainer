using System;
using System.Collections.Generic;
#if MAUI
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
#if __ANDROID__
using APES.UI.XF.Droid;
using Microsoft.Maui.Controls.Compatibility.Hosting;
#elif __IOS__ || __MACCATALYST__
using APES.UI.XF.iOS;
#elif NET6_0_WINDOWS10_0_17763_0
using APES.UI.XF.UWP;
using Microsoft.Maui.Controls.Compatibility.Hosting;
#endif
#endif

namespace APES.UI.XF
{
    public static class ContextMenuContainerExtensions
    {
        public static bool HasMenuOptions(this ContextMenuContainer container) => container.MenuItems.Count > 0;
#if MAUI
        public static MauiAppBuilder ConfigureContextMenuContainer(this MauiAppBuilder mauiAppBuilder)
        {

#if __ANDROID__ || NET6_0_WINDOWS10_0_17763_0
	    mauiAppBuilder.UseMauiCompatibility();  
#endif          
            return mauiAppBuilder.ConfigureMauiHandlers(handlers =>
             {

#if __ANDROID__
                handlers.AddCompatibilityRenderer<ContextMenuContainer, ContextMenuContainerRenderer>();
                
#elif __IOS__ || __MACCATALYST__
                handlers.AddHandler<ContextMenuContainer, ContextMenuContainerRenderer>();
#elif NET6_0_WINDOWS10_0_17763_0
                 handlers.AddCompatibilityRenderer<ContextMenuContainer, ContextMenuContainerRenderer>();
#endif
             });
        }
#endif
    }
}
