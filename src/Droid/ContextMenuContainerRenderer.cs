// MIT License
// Copyright (c) 2021 Pavel Anpin

#pragma warning disable SA1137
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using AndroidX.AppCompat.Widget;
using Java.Lang.Reflect;
using AColor = Android.Graphics.Color;
using Path = System.IO.Path;
using DrawableWrapperX =  Android.Graphics.Drawables.InsetDrawable;
using Android.Runtime;
using Android.Util;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace APES.MAUI;

internal sealed class ContextMenuContainerRenderer : ContentViewHandler
{
    protected override void DisconnectHandler(ContentViewGroup platformView)
    {
        if (VirtualView is ContextMenuContainer old)
        {
            old.BindingContextChanged -= Element_BindingContextChanged;
            if (old.MenuItems != null)
            {
                old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;
            }
        }

        base.DisconnectHandler(platformView);
    }

    protected override void ConnectHandler(ContentViewGroup platformView)
    {
        if (VirtualView is ContextMenuContainer newElement)
        {
            newElement.BindingContextChanged += Element_BindingContextChanged;
            if (newElement.MenuItems is not null)
            {
                foreach (var element in newElement.MenuItems)
                {
                    element.PropertyChanged += Item_Changed;
                }
                newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
            }

            RefillMenuItems();
        }

        base.ConnectHandler(platformView);
    }

    protected override ContentViewGroup CreatePlatformView()
    {
        if (VirtualView is null)
        {
            throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a ContentViewGroup");
        }

        if (VirtualView is not ContextMenuContainer)
        {
            throw new InvalidOperationException($"{nameof(VirtualView)} must be of type ContextMenuContainer, but was {VirtualView.GetType()} ");
        }

        var viewGroup = new ContainerViewGroup(Context);
        return viewGroup;
    }

    private void RefillMenuItems()
    {
        if (VirtualView is ContextMenuContainer container)
        {
            ConstructInteraction(container);
        }
    }

    private void MenuItems_CollectionChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (BaseContextMenuItem item in e.OldItems)
            {
                item.PropertyChanged -= Item_Changed;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (BaseContextMenuItem item in e.NewItems)
            {
                item.PropertyChanged += Item_Changed;
            }
        }

        RefillMenuItems();
    }

    private void Item_Changed(object? sender, PropertyChangedEventArgs e) => ((ContainerViewGroup)PlatformView).NeedToRefillMenu = true;

    private void Element_BindingContextChanged(object? sender, EventArgs e)
        => RefillMenuItems();

    private void ConstructInteraction(ContextMenuContainer menuItems) =>
        ((ContainerViewGroup)PlatformView).SetupMenu(menuItems);

    private class ContainerViewGroup : ContentViewGroup
    {
#pragma warning disable SA1306
#pragma warning disable SX1309
        // ReSharper disable once InconsistentNaming
        private ContextMenuContainer? Element;
#pragma warning restore SX1309
#pragma warning restore SA1306

        public ContainerViewGroup(Context context)
            : base(context)
        {
        }

        // ReSharper disable once UnusedMember.Local
        public ContainerViewGroup(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        // ReSharper disable once UnusedMember.Local
        public ContainerViewGroup(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        // ReSharper disable once UnusedMember.Local
        public ContainerViewGroup(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
        }

        // ReSharper disable once UnusedMember.Local
        public ContainerViewGroup(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public void SetupMenu(ContextMenuContainer? container)
        {
            DeconstructInteraction();
            Element = container;
        }
            
#pragma warning disable SA1201
        private PopupMenu? _contextMenu;
#pragma warning restore SA1201
        private MyTimer? _timer;
        private bool _timerFired;

        // ReSharper disable once RedundantTypeCheckInPattern
        private bool ContextMenuIsNotEmpty => Element is ContextMenuContainer {MenuItems.Count: > 0};

        public override bool DispatchTouchEvent(MotionEvent? e)
        {
            if (e == null)
            {
                return base.DispatchTouchEvent(e);
            }

            bool result;
            Logger.Debug("ContextMenuContainer DispatchTouchEvent fired {0}", e.Action);
            if (ContextMenuIsNotEmpty && e.Action == MotionEventActions.Down)
            {
                // You can change the timespan of the long press
                _timerFired = false;
                _timer = new MyTimer(
                    TimeSpan.FromMilliseconds(1500),
                    () =>
                    {
                        _timerFired = true;
                        OpenContextMenu();
                    });
                _timer.Start();
            }

            if (_timerFired)
            {
                result = true;
            }
            else if (e.Action is MotionEventActions.Up or MotionEventActions.Cancel)
            {
                _timer?.Stop();
                result = base.DispatchTouchEvent(e);
            }
            else
            {
                result = base.DispatchTouchEvent(e);

                // ReSharper disable once ConvertIfToOrExpression
                if (!result && ContextMenuIsNotEmpty)
                {
                    result = true;
                }
            }

            return result;
        }

        private void DeconstructInteraction()
        {
            if (Element is not null && _contextMenu is not null)
            {
                _contextMenu.Dismiss();
                _contextMenu.Menu.Clear();
            }
        }

        private void OpenContextMenu()
        {
            if (GetContextMenu() is null)
            {
                ConstructNativeMenu();
                FillMenuItems();
            }

            _contextMenu?.Show();
        }

        private void ConstructNativeMenu()
        {
            var child = GetChildAt(0);
            if (child is null)
            {
                return;
            }

            _contextMenu = new PopupMenu(Context, child);
            _contextMenu.MenuItemClick += ContextMenu_MenuItemClick;
            
            // Enable group dividers for separators (API 28+)
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.P)
            {
                _contextMenu.Menu.SetGroupDividerEnabled(true);
            }
    
            Field field = _contextMenu.Class.GetDeclaredField("mPopup");
            field.Accessible = true;
            Java.Lang.Object? menuPopupHelper = field.Get(_contextMenu);
            Method? setForceIcons =
                menuPopupHelper?.Class.GetDeclaredMethod("setForceShowIcon", Java.Lang.Boolean.Type!);
            setForceIcons?.Invoke(menuPopupHelper, true);
        }

        private void DeconstructNativeMenu()
        {
            if (_contextMenu is null)
            {
                return;
            }

            _contextMenu.MenuItemClick -= ContextMenu_MenuItemClick;
            _contextMenu.Dispose();
            _contextMenu = null;
            NeedToRefillMenu = false;
        }

        private void AddMenuItem(BaseContextMenuItem item)
        {
            if (_contextMenu is null)
            {
                return;
            }

            switch (item)
            {
                // Separator Items
                case ContextMenuSeparator:
                {
                    // For API < 28, create a disabled item as a visual separator
                    var separator = _contextMenu.Menu.Add("────────");
                    separator?.SetEnabled(false);
                    break;
                }

                // Normal Items
                case ContextMenuItem contextItem:
                {
                    if (string.IsNullOrEmpty(contextItem.Text))
                    {
                        Logger.Error("ContextMenuItem text should not be empty!");
                        break;
                    }

                    var title = new SpannableString(contextItem.Text);
                    if (contextItem.IsDestructive)
                    {
                        title.SetSpan(new ForegroundColorSpan(AColor.Red), 0, title.Length(), 0);
                    }

                    var contextAction = _contextMenu.Menu.Add(title);
                    if (contextAction is null)
                    {
                        Logger.Error("We couldn't create IMenuItem with title {0}", contextItem.Text);
                        break;
                    }

                    contextAction.SetEnabled(contextItem.IsEnabled);
                    if (contextItem.Icon != null && !string.IsNullOrWhiteSpace(contextItem.Icon.File))
                    {
                        string name = Path.GetFileNameWithoutExtension(contextItem.Icon.File);
                        int id = Context?.GetDrawableId(name) ?? 0;
                        if (id == 0)
                        {
                            break;
                        }

                        var drawable = Context?.GetDrawable(id);
                        if (drawable is not null)
                        {
                            var wrapper = new DrawableWrapperX(drawable, 0);
                            if (contextItem.IsDestructive)
                            {
                                wrapper.SetTint(AColor.Red);
                            }

                            contextAction.SetIcon(wrapper);
                        }
                    }

                    break;
                }
            }
        }

        private void FillMenuItems()
        {
            // ReSharper disable once RedundantTypeCheckInPattern
            if (Element is ContextMenuContainer {MenuItems.Count: > 0} element)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.P)
                {
                    // Use group-based separators for API 28+
                    int currentGroupId = 0;
            
                    foreach (var item in element.MenuItems)
                    {
                        if (item is ContextMenuSeparator)
                        {
                            // Move to next group - this creates the visual separator
                            currentGroupId++;
                        }
                        else if (item is ContextMenuItem contextItem)
                        {
                            AddMenuItemToGroup(contextItem, currentGroupId);
                        }
                    }
                }
                else
                {
                    // Fallback for API < 28
                    foreach (var item in element.MenuItems)
                    {
                        AddMenuItem(item);
                    }
                }
            }
        }
        
        private void AddMenuItemToGroup(ContextMenuItem contextItem, int groupId)
        {
            if (_contextMenu is null)
            {
                return;
            }

            if (string.IsNullOrEmpty(contextItem.Text))
            {
                Logger.Error("ContextMenuItem text should not be empty!");
                return;
            }

            var title = new SpannableString(contextItem.Text);
            if (contextItem.IsDestructive)
            {
                title.SetSpan(new ForegroundColorSpan(AColor.Red), 0, title.Length(), 0);
            }

            // Add item to specific group
            var contextAction = _contextMenu.Menu.Add(groupId, Menu.None, Menu.None, title);
            if (contextAction is null)
            {
                Logger.Error("We couldn't create IMenuItem with title {0}", contextItem.Text);
                return;
            }

            contextAction.SetEnabled(contextItem.IsEnabled);
            if (contextItem.Icon != null && !string.IsNullOrWhiteSpace(contextItem.Icon.File))
            {
                string name = Path.GetFileNameWithoutExtension(contextItem.Icon.File);
                int id = Context?.GetDrawableId(name) ?? 0;
                if (id == 0)
                {
                    return;
                }

                var drawable = Context?.GetDrawable(id);
                if (drawable is not null)
                {
                    var wrapper = new DrawableWrapperX(drawable, 0);
                    if (contextItem.IsDestructive)
                    {
                        wrapper.SetTint(AColor.Red);
                    }

                    contextAction.SetIcon(wrapper);
                }
            }
        }

#pragma warning disable SA1137
        private PopupMenu? GetContextMenu()
#pragma warning restore SA1137
        {
            // ReSharper disable once ConvertTypeCheckPatternToNullCheck
            if (_contextMenu != null && Element is ContextMenuContainer element)
            {
                if (NeedToRefillMenu || element.MenuItems?.Count != _contextMenu.Menu.Size())
                {
                    DeconstructNativeMenu();
                    
                }
            }

            return _contextMenu;
        }

        private void ContextMenu_MenuItemClick(object? sender, PopupMenu.MenuItemClickEventArgs e)
        {
            // ReSharper disable once RedundantCast
            var item = ((ContextMenuContainer?)Element)?.MenuItems?
                .Where(x => x is ContextMenuItem)
                .Select(x => (ContextMenuItem)x)
                .FirstOrDefault(x => x.Text == e.Item?.TitleFormatted?.ToString());

            item?.OnItemTapped();
        }

        public bool NeedToRefillMenu { get; set; } = false;
    }

    private class MyTimer
    {
        private readonly TimeSpan _timespan;
        private readonly Action _callback;

        private CancellationTokenSource _cancellation;

        public MyTimer(TimeSpan timespan, Action callback)
        {
            _timespan = timespan;
            _callback = callback;
            _cancellation = new CancellationTokenSource();
        }

        public void Start()
        {
            CancellationTokenSource cts = _cancellation; // safe copy
            DispatcherProvider.Current.GetForCurrentThread() !.StartTimer(

#pragma warning disable SA1114
                interval: _timespan,
#pragma warning restore SA1114
                callback: () =>
                {
                    if (cts.IsCancellationRequested)
                    {
                        return false;
                    }

                    _callback.Invoke();
                    return false; // or true for periodic behavior
                });
        }

        public void Stop() => Interlocked.Exchange(ref _cancellation, new CancellationTokenSource()).Cancel();
    }
}

#pragma warning restore SA1137
