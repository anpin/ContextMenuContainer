using System;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Text;
using Android.Text.Style;
using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;
using DrawableWrapperX = AndroidX.AppCompat.Graphics.Drawable.DrawableWrapper;
using Java.Lang.Reflect;
#if MAUI
using Java.Interop;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Handlers;
#else
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
#endif
using APES.UI.XF;
using APES.UI.XF.Droid;
using Path = System.IO.Path;
using AColor = Android.Graphics.Color;
using AView = Android.Views.View;
#if !MAUI
[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
#endif

namespace APES.UI.XF.Droid
{
    [Preserve(AllMembers = true)]
    public class ContextMenuContainerRenderer
#if MAUI
        : ViewHandler<ContextMenuContainer, AView>
#else
        : ViewRenderer
#endif
    {
        PopupMenu? contextMenu;

#if MAUI
        public ContextMenuContainerRenderer(IPropertyMapper mapper, CommandMapper? commandMapper) : base(mapper,
            commandMapper)
        {

        }

        protected override AView CreatePlatformView()
        {
            var v = new AView(Context);
            v.SetOnTouchListener(new TouchListner(() => enabled, OpenContextMenu));
            return v;
        }

        public override void SetVirtualView(IView view)
        {
            if (VirtualView is ContextMenuContainer old)
            {
                old.BindingContextChanged -= Element_BindingContextChanged;
                old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;

            }

            if (view is ContextMenuContainer newElement)
            {
                newElement.BindingContextChanged += Element_BindingContextChanged;
                newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;

            }

            base.SetVirtualView(view);
        }
#else
        public ContextMenuContainerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement is ContextMenuContainer old)
            {
                old.BindingContextChanged -= Element_BindingContextChanged;
                old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;
            }
            if (e.NewElement is ContextMenuContainer newElement)
            {

                newElement.BindingContextChanged += Element_BindingContextChanged;
                newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
            }
        }
#endif
        void ConstructNativeMenu()
        {
#if MAUI
            var child = ((ViewGroup?) PlatformView.Parent)?.GetChildAt(0);
#else
            var child = GetChildAt(0);
#endif
            if (child == null)
                return;
            contextMenu = new PopupMenu(Context, child);
            contextMenu.MenuItemClick += ContextMenu_MenuItemClick;
            Field field = contextMenu.Class.GetDeclaredField("mPopup");
            field.Accessible = true;
            Java.Lang.Object? menuPopupHelper = field.Get(contextMenu);
            Method? setForceIcons =
                menuPopupHelper?.Class.GetDeclaredMethod("setForceShowIcon", Java.Lang.Boolean.Type);
            setForceIcons?.Invoke(menuPopupHelper, true);
        }

        void DeconstructNativeMenu()
        {
            if (contextMenu == null)
                return;
            contextMenu.MenuItemClick -= ContextMenu_MenuItemClick;
            contextMenu.Dispose();
            contextMenu = null;
        }

        void AddMenuItem(ContextMenuItem item)
        {
            if (contextMenu == null)
                return;
            var title = new SpannableString(item.Text);
            if (item.IsDestructive)
                title.SetSpan(new ForegroundColorSpan(AColor.Red), 0, title.Length(), 0);
            var contextAction = contextMenu.Menu.Add(title);
            if (contextAction == null)
            {
                Logger.Error("We couldn't create IMenuItem with title {0}", item.Text);
                return;
            }

            contextAction.SetEnabled(item.IsEnabled);
            if (item.Icon != null)
            {
                var name = Path.GetFileNameWithoutExtension(item.Icon.File);
                var id = Context.GetDrawableId(name);
                if (id != 0)
                {
                    Drawable? drawable = (int) Build.VERSION.SdkInt >= 21
                        ? Context?.GetDrawable(id)
                        : Context?.GetDrawable(name);
                    if (drawable != null)
                    {
                        var wrapper = new DrawableWrapperX(drawable);
                        if (item.IsDestructive)
                            wrapper.SetTint(AColor.Red);
                        contextAction.SetIcon(wrapper);
                    }
                }
            }
        }

        void FillMenuItems()
        {
#if MAUI
            if (VirtualView is ContextMenuContainer element)
#else
            if (Element is ContextMenuContainer element)
#endif

            {
                if (element.MenuItems.Count > 0)
                {
                    foreach (var item in element.MenuItems)
                    {
                        AddMenuItem(item);
                    }
                }
            }
        }

        void RefillMenuItems()
        {
            if (contextMenu == null)
                return;
            contextMenu.Dismiss();
            contextMenu.Menu.Clear();
            FillMenuItems();
        }

        PopupMenu? GetContextMenu()
        {
#if MAUI
            if (contextMenu != null && VirtualView is ContextMenuContainer element)
#else
            if (contextMenu != null && Element is ContextMenuContainer element)
#endif
            {
                if (element.MenuItems.Count != contextMenu.Menu.Size())
                {
                    DeconstructNativeMenu();
                }
                else
                {
                    for (int i = 0; i < contextMenu.Menu.Size(); i++)
                    {
                        if (!element.MenuItems[i].Text.Equals(contextMenu.Menu.GetItem(i)?.TitleFormatted?.ToString()))
                        {
                            DeconstructNativeMenu();
                            break;
                        }
                    }
                }
            }

            return contextMenu;
        }

        private void MenuItems_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefillMenuItems();
        }

        private void Element_BindingContextChanged(object sender, EventArgs e)
        {
            RefillMenuItems();
        }

        private void ContextMenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
#if MAUI
            var virt = ((ContextMenuContainer) VirtualView);
#else
            var virt = ((ContextMenuContainer)Element);
#endif
            var item = virt.MenuItems.FirstOrDefault(x => x.Text == e.Item.TitleFormatted?.ToString());
            item?.OnItemTapped();
        }
#if MAUI
        bool enabled => VirtualView is ContextMenuContainer element && element.MenuItems.Count > 0;
#else
        bool enabled => Element is ContextMenuContainer element && element.MenuItems.Count > 0;
#endif
        const int LongPressWaitTime = 1500;
#if MAUI
        class TouchListner : Java.Lang.Object, AView.IOnTouchListener
        {
            readonly Func<bool> _enabled;
            readonly Action _onHold;

            MyTimer timer;
            bool timerFired = false;

            public TouchListner(Func<bool> enabled, Action onHold)
            {
                _enabled = enabled;
                _onHold = onHold;
            }
            public bool OnTouch(AView? v, MotionEvent? e)
            {
                bool result = false;
                Logger.Debug("TouchListner OnTouch fired {0}", e.Action);
                if (_enabled() && e.Action == MotionEventActions.Down)
                {
                    //You can change the timespan of the long press
                    timerFired = false;
                    timer = new MyTimer(TimeSpan.FromMilliseconds(LongPressWaitTime), () =>
                    {
                        timerFired = true;
                        _onHold.Invoke();
                    });
                    timer.Start();
                }

                if (timerFired)
                {
                    result = true;
                }
                else if (e.Action == MotionEventActions.Up || e.Action == MotionEventActions.Cancel)
                {
                    timer?.Stop();
                    result = false;
                }
                return result;
            }
        }
#else
        MyTimer timer;
        bool timerFired = false;
        public override bool DispatchTouchEvent(MotionEvent e)
        {
                bool result;
                Logger.Debug("ContextMenuContainer DispatchTouchEvent fired {0}", e.Action);
                if (enabled && e.Action == MotionEventActions.Down)
                {
                    //You can change the timespan of the long press
                    timerFired = false;
                    timer = new MyTimer(TimeSpan.FromMilliseconds(LongPressWaitTime), () =>
                    {
                        timerFired = true;
                        OpenContextMenu();
                    });
                    timer.Start();
                }

                if (timerFired)
                {
                    result = true;
                }
                else if (e.Action == MotionEventActions.Up || e.Action == MotionEventActions.Cancel)
                {
                    timer?.Stop();
                    result = base.DispatchTouchEvent(e);
                }
                else
                {
                    result = base.DispatchTouchEvent(e);
                    if (!result && enabled)
                    {
                        result = true;
                    }
                }

                return result;
            }
#endif
        void OpenContextMenu()
        {
            if (GetContextMenu() == null)
            {
                ConstructNativeMenu();
                FillMenuItems();

            }

            contextMenu?.Show();
        }

        class MyTimer
        {
            private readonly TimeSpan timespan;
            private readonly Action callback;

            private CancellationTokenSource cancellation;

            public MyTimer(TimeSpan timespan, Action callback)
            {
                this.timespan = timespan;
                this.callback = callback;
                this.cancellation = new CancellationTokenSource();
            }

            public void Start()
            {
                CancellationTokenSource cts = this.cancellation; // safe copy
                Device.StartTimer(this.timespan,
                    () =>
                    {
                        if (cts.IsCancellationRequested) return false;
                        this.callback.Invoke();
                        return false; // or true for periodic behavior
                    });
            }

            public void Stop()
            {
                Interlocked.Exchange(ref this.cancellation, new CancellationTokenSource()).Cancel();
            }
        }
    }
}
