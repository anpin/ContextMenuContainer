using System;
using System.Collections.Generic;
#if MAUI
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Hosting;
#if __ANDROID__
//using APES.UI.XF.Droid;
#elif __IOS__ || __MACCATALYST__
using APES.UI.XF.iOS;
#elif __WINDOWS__
using APES.UI.XF.UWP;
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
            
            return mauiAppBuilder.ConfigureMauiHandlers(handlers =>
             {

#if __ANDROID__
                handlers.AddHandler<ContextMenuContainer, ContextMenuContainerHandler>();
                
#elif __IOS__ || __MACCATALYST__
                 handlers.AddHandler<ContextMenuContainer, ContextMenuContainerHandler>();
#elif __WINDOWS__
                 handlers.AddCompatibilityRenderer<ContextMenuContainer, ContextMenuContainerRenderer>();
#endif
             });
        }
#endif
    }
}
