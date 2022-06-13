// MIT License
// Copyright (c) 2021 Pavel Anpin

using System.Collections.Generic;
#if MAUI
using Microsoft.Maui.Controls;

#else
using Xamarin.Forms;

#endif
namespace APES.UI.XF
{
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

        /// <summary>
        /// Call this in order to preserve our code during linking and allow namespace resolution in XAML.
        /// </summary>
        public static void Init()
        {
            // maybe do something here later
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (MenuItems != null)
            {
                SetBindingContextForItems(MenuItems);
            }
        }

        private static object DefaultMenuItemsCreator(BindableObject bindableObject)
        {
            var menuItems = new ContextMenuItems();
            menuItems.CollectionChanged += (_, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (ContextMenuItem item in e.OldItems)
                    {
                        item.RemoveBinding(BindingContextProperty);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (ContextMenuItem item in e.NewItems)
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
                foreach (ContextMenuItem item in oldItems)
                {
                    item.RemoveBinding(BindingContextProperty);
                }

                // oldItems.CollectionChanged -= MenuItems_CollectionChanged;
            }

            if (newValue is ContextMenuItems newItems)
            {
                foreach (ContextMenuItem item in newItems)
                {
                    SetInheritedBindingContext(item, bindableObject.BindingContext);
                }
            }
        }

        private void SetBindingContextForItems(IList<ContextMenuItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                SetBindingContextForItem(items[i]);
            }
        }

        private void SetBindingContextForItem(ContextMenuItem item) => SetInheritedBindingContext(item, BindingContext);
    }
}
