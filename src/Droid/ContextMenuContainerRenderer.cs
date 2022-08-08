// MIT License
// Copyright (c) 2021 Pavel Anpin

#pragma warning disable SA1137
using System;
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
using DrawableWrapperX = AndroidX.AppCompat.Graphics.Drawable.DrawableWrapper;
using Path = System.IO.Path;
#if MAUI
using Android.Runtime;
using Android.Util;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace APES.UI.XF
{
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
                if (newElement.MenuItems != null)
                {
                    newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
                }

                RefillMenuItems();
            }

            base.ConnectHandler(platformView);
        }

        protected override ContentViewGroup CreatePlatformView()
        {
            if (VirtualView == null)
            {
                throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a ContentViewGroup");
            }

            if (VirtualView is not ContextMenuContainer)
            {
                throw new InvalidOperationException(
                    $"{nameof(VirtualView)} must be of type ContextMenuContainer, but was {VirtualView.GetType()} ");
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
            => RefillMenuItems();

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
#else
using Android.OS;
using APES.UI.XF;
using APES.UI.XF.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using PreserveAttribute = Xamarin.Forms.Internals.PreserveAttribute;
using XView = Xamarin.Forms.View;

[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]

namespace APES.UI.XF.Droid
{
    [Preserve(AllMembers = true)]
    internal class ContextMenuContainerRenderer : ViewRenderer
    {
        public ContextMenuContainerRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement is ContextMenuContainer old)
            {
                old.BindingContextChanged -= Element_BindingContextChanged;
                if (old.MenuItems != null)
                {
                    old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;
                }
            }

            if (e.NewElement is ContextMenuContainer newElement)
            {
                newElement.BindingContextChanged += Element_BindingContextChanged;
                if (newElement.MenuItems != null)
                {
                    newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
                }
            }
        }

        private void ConstructInteraction(ContextMenuContainer container)
        {
            DeconstructInteraction();
            if (container.MenuItems?.Count > 0)
            {
                foreach (var item in container.MenuItems)
                {
                    AddMenuItem(item);
                }
            }
        }

        private void RefillMenuItems()
        {
            if (Element is ContextMenuContainer container)
            {
                ConstructInteraction(container);
            }
        }

        private void MenuItems_CollectionChanged(
            object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            => RefillMenuItems();

        private void Element_BindingContextChanged(object sender, EventArgs e)
            => RefillMenuItems();
#endif

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
                if (Element != null && _contextMenu != null)
                {
                    _contextMenu.Dismiss();
                    _contextMenu.Menu.Clear();
                }
            }

            private void OpenContextMenu()
            {
                if (GetContextMenu() == null)
                {
                    ConstructNativeMenu();
                    FillMenuItems();
                }

                _contextMenu?.Show();
            }

            private void ConstructNativeMenu()
            {
                var child = GetChildAt(0);
                if (child == null)
                {
                    return;
                }

                _contextMenu = new PopupMenu(Context, child);
                _contextMenu.MenuItemClick += ContextMenu_MenuItemClick;
                Field field = _contextMenu.Class.GetDeclaredField("mPopup");
                field.Accessible = true;
                Java.Lang.Object? menuPopupHelper = field.Get(_contextMenu);
                Method? setForceIcons =
                    menuPopupHelper?.Class.GetDeclaredMethod("setForceShowIcon", Java.Lang.Boolean.Type!);
                setForceIcons?.Invoke(menuPopupHelper, true);
            }

            private void DeconstructNativeMenu()
            {
                if (_contextMenu == null)
                {
                    return;
                }

                _contextMenu.MenuItemClick -= ContextMenu_MenuItemClick;
                _contextMenu.Dispose();
                _contextMenu = null;
            }

            private void AddMenuItem(ContextMenuItem item)
            {
                if (_contextMenu == null)
                {
                    return;
                }

                var title = new SpannableString(item.Text);
                if (item.IsDestructive)
                {
                    title.SetSpan(new ForegroundColorSpan(AColor.Red), 0, title.Length(), 0);
                }

                var contextAction = _contextMenu.Menu.Add(title);
                if (contextAction == null)
                {
                    Logger.Error("We couldn't create IMenuItem with title {0}", item.Text);
                    return;
                }

                contextAction.SetEnabled(item.IsEnabled);
                if (item.Icon != null)
                {
                    string name = Path.GetFileNameWithoutExtension(item.Icon.File);
                    int id = Context?.GetDrawableId(name) ?? 0;
                    if (id == 0)
                    {
                        return;
                    }
#if MAUI
                    Drawable? drawable = Context?.GetDrawable(id);
#else
                Drawable? drawable = (int)Build.VERSION.SdkInt >= 21
                    ? Context?.GetDrawable(id)
                    : Context?.GetDrawable(name);
#endif
                    if (drawable != null)
                    {
                        var wrapper = new DrawableWrapperX(drawable);
                        if (item.IsDestructive)
                        {
                            wrapper.SetTint(AColor.Red);
                        }

                        contextAction.SetIcon(wrapper);
                    }
                }
            }

            private void FillMenuItems()
            {
                // ReSharper disable once RedundantTypeCheckInPattern
                if (Element is ContextMenuContainer {MenuItems.Count: > 0} element)
                {
                    foreach (var item in element.MenuItems)
                    {
                        AddMenuItem(item);
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
                    if (element.MenuItems?.Count != _contextMenu.Menu.Size())
                    {
                        DeconstructNativeMenu();
                    }
                    else
                    {
                        for (int i = 0; i < _contextMenu.Menu.Size(); i++)
                        {
                            if (!element.MenuItems[i].Text
                                    .Equals(_contextMenu.Menu.GetItem(i)?.TitleFormatted?.ToString()))
                            {
                                DeconstructNativeMenu();
                                break;
                            }
                        }
                    }
                }

                return _contextMenu;
            }

            private void ContextMenu_MenuItemClick(object? sender, PopupMenu.MenuItemClickEventArgs e)
            {
                // ReSharper disable once RedundantCast
                var item = ((ContextMenuContainer?)Element)?.MenuItems?.FirstOrDefault(
                    x => x.Text == e.Item.TitleFormatted?.ToString());
                item?.OnItemTapped();
            }

#if MAUI
        }

#endif
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
#if MAUI
                DispatcherProvider.Current.GetForCurrentThread() !.StartTimer(
#else
                Device.StartTimer(
#endif
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
}

#pragma warning restore SA1137
