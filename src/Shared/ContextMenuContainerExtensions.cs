// MIT License
// Copyright (c) 2021 Pavel Anpin

#if MAUI && (__ANDROID__ || __IOS__ || __MACCATALYST__ || __WINDOWS__)
using Microsoft.Maui.Hosting;
#endif

namespace APES.UI.XF
{
    public static class ContextMenuContainerExtensions
    {
        public static bool HasMenuOptions(this ContextMenuContainer container) => container.MenuItems?.Count > 0;

#if MAUI && (__ANDROID__ || __IOS__ || __MACCATALYST__ || __WINDOWS__)
        public static MauiAppBuilder ConfigureContextMenuContainer(this MauiAppBuilder mauiAppBuilder) =>
            mauiAppBuilder.ConfigureMauiHandlers(handlers => handlers.AddHandler<ContextMenuContainer, ContextMenuContainerRenderer>());
#endif
    }
}
