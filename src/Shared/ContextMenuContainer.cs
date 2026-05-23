// MIT License
// Copyright (c) 2021 Pavel Anpin

using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace APES.MAUI;

public class ContextMenuContainer : ContentView
{
    public static readonly BindableProperty MenuItemsProperty =
        BindableProperty.Create(
            nameof(MenuItems),
            typeof(ContextMenuItems),
            typeof(VisualElement),
            defaultValueCreator: DefaultMenuItemsCreator,
            propertyChanged: OnMenuItemsChanged);

    public ContextMenuItems? MenuItems
    {
        get => (ContextMenuItems?)GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (MenuItems is not null)
        {
            SetBindingContextForItems(MenuItems);
        }
    }

    private static object DefaultMenuItemsCreator(BindableObject bindableObject)
    {
        var menuItems = new ContextMenuItems();
        menuItems.CollectionChanged += (_, e) =>
        {
            if (e.OldItems is not null)
            {
                foreach (BindableObject item in e.OldItems)
                {
                    item.RemoveBinding(BindingContextProperty);
                }
            }

            if (e.NewItems is not null)
            {
                foreach (BindableObject item in e.NewItems)
                {
                    SetInheritedBindingContext(item, bindableObject.BindingContext);
                }
            }
        };
        return menuItems;
    }

    private static void OnMenuItemsChanged(BindableObject bindableObject, object newValue, object oldValue)
    {
        if (oldValue is ContextMenuItems oldItems)
        {
            foreach (var item in oldItems)
            {
                item.RemoveBinding(BindingContextProperty);
            }

            // oldItems.CollectionChanged -= MenuItems_CollectionChanged;
        }

        if (newValue is ContextMenuItems newItems)
        {
            foreach (var item in newItems)
            {
                SetInheritedBindingContext(item, bindableObject.BindingContext);
            }
        }
    }

    private void SetBindingContextForItems(IList<BaseContextMenuItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            SetBindingContextForItem(items[i]);
        }
    }

    private void SetBindingContextForItem(BaseContextMenuItem item) => SetInheritedBindingContext(item, BindingContext);
}
