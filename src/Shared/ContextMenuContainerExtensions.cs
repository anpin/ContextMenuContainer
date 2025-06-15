// MIT License
// Copyright (c) 2021 Pavel Anpin


using Microsoft.Maui.Hosting;

namespace APES.MAUI;

public static class ContextMenuContainerExtensions
{
    public static bool HasMenuOptions(this ContextMenuContainer container) => container.MenuItems?.Count > 0;

    public static MauiAppBuilder ConfigureContextMenuContainer(this MauiAppBuilder mauiAppBuilder) 
    {
#if (__ANDROID__ || __IOS__ || __MACCATALYST__ || __WINDOWS__)
        mauiAppBuilder.ConfigureMauiHandlers(handlers => handlers.AddHandler<ContextMenuContainer, ContextMenuContainerRenderer>());
#endif
        return mauiAppBuilder;
    }
}
