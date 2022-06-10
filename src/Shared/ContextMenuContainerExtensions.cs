using System;
using System.Collections.Generic;
#if MAUI
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Hosting;
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
                 handlers.AddHandler<ContextMenuContainer, ContextMenuContainerRenderer>();

             });
        }
#endif
    }
}
