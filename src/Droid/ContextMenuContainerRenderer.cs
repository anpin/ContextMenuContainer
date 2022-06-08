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
using Android.Runtime;
using Android.Util;
using AndroidX.AppCompat.Widget;
using DrawableWrapperX = AndroidX.AppCompat.Graphics.Drawable.DrawableWrapper;
using Java.Lang.Reflect;
#if MAUI
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using JetBrains.Annotations;
#else
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using APES.UI.XF.Droid;
using XView = Xamarin.Forms.View;
using PreserveAttribute = Xamarin.Forms.Internals.PreserveAttribute;
#endif
using APES.UI.XF;
using Path = System.IO.Path;
using AColor = Android.Graphics.Color;
#if MAUI
namespace APES.UI.XF
#else
[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
namespace APES.UI.XF.Droid
#endif
{
#if MAUI
    sealed partial class ContextMenuContainerHandler : ContentViewHandler
#else
    [Preserve(AllMembers = true)]
    class ContextMenuContainerRenderer : ViewRenderer
#endif

    {
#if MAUI
        //private IContentView Element => VirtualView;
        
        void constructInteraction(ContextMenuContainer menuItems)
        {
            ((ContainerViewGroup)PlatformView).SetupMenu(menuItems);
            //deconstructIntercation();
            //if (menuItems?.Count > 0)
            //{
            //    foreach (var item in menuItems)
            //    {
            //        AddMenuItem(item);
            //    }
            //}
        }
        
        protected override ContentViewGroup CreatePlatformView()
        {
            if (VirtualView == null)
            {
                throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a ContentViewGroup");
            }
            if (VirtualView is not ContextMenuContainer)
            {
                throw new InvalidOperationException($"{nameof(VirtualView)} must be of type ContextMenuContainer, but was {VirtualView.GetType()} ");
            }

            var viewGroup = new ContainerViewGroup(Context);
            //{
            //    CrossPlatformMeasure = VirtualView.CrossPlatformMeasure,
            //    CrossPlatformArrange = VirtualView.CrossPlatformArrange
            //};
            return viewGroup;
        }
#else
        public ContextMenuContainerRenderer(Context context) : base(context)
        {

        }
        
        protected override void OnElementChanged(ElementChangedEventArgs<XView> e)
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
        void constructInteraction(ContextMenuContainer container)
        {
            deconstructIntercation();
            if (container.MenuItems?.Count > 0)
            {
                foreach (var item in container.MenuItems)
                {
                    AddMenuItem(item);
                }
            }
        }

        void RefillMenuItems()
        {
            if (Element is ContextMenuContainer container)
            {
                constructInteraction(container);
            }
        }
        void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefillMenuItems();
        }

        void Element_BindingContextChanged(object sender, EventArgs e)
        {
            RefillMenuItems();
        }
#endif


#if MAUI

        class ContainerViewGroup : ContentViewGroup
        {
            

            private ContextMenuContainer? Element;
#endif

        PopupMenu? contextMenu;
        MyTimer? timer;
        bool timerFired = false;

        private bool enabled => Element is ContextMenuContainer element && element.MenuItems.Count > 0;
#if MAUI
            public ContainerViewGroup([NotNull] Context context) : base(context)
            {

            }

            public ContainerViewGroup(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public ContainerViewGroup([NotNull] Context context, [NotNull] IAttributeSet attrs) : base(context, attrs)
            {
            }

            public ContainerViewGroup([NotNull] Context context, [NotNull] IAttributeSet attrs, int defStyleAttr) :
                base(context, attrs, defStyleAttr)
            {
            }

            public ContainerViewGroup([NotNull] Context context, [NotNull] IAttributeSet attrs, int defStyleAttr,
                int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
            {
            }

            public void SetupMenu(ContextMenuContainer? container)
            {
                deconstructIntercation();
                Element = container;
            }
#endif


        void deconstructIntercation()
            {
                if (Element != null && contextMenu != null)
                {
                    contextMenu.Dismiss();
                    contextMenu.Menu.Clear();
                    //contextMenuDelegate?.Dispose();
                    //contextMenu?.Dispose();
                }
            }
            public override bool DispatchTouchEvent(MotionEvent e)
            {
                bool result;
                Logger.Debug("ContextMenuContainer DispatchTouchEvent fired {0}", e.Action);
                if (enabled && e.Action == MotionEventActions.Down)
                {
                    //You can change the timespan of the long press
                    timerFired = false;
                    timer = new MyTimer(TimeSpan.FromMilliseconds(1500), () =>
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



            void OpenContextMenu()
            {
                if (GetContextMenu() == null)
                {
                    ConstructNativeMenu();
                    FillMenuItems();

                }

                contextMenu?.Show();
            }

            void ConstructNativeMenu()
            {
                var child = GetChildAt(0);
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
                                wrapper.SetTint(AColor.Red);
                            contextAction.SetIcon(wrapper);
                        }
                    }
                }
            }

            void FillMenuItems()
            {
                if (Element is ContextMenuContainer element)
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

            PopupMenu? GetContextMenu()
            {
                if (contextMenu != null && Element is ContextMenuContainer element)
                {
                    if (element.MenuItems.Count != contextMenu.Menu.Size())
                    {
                        DeconstructNativeMenu();
                    }
                    else
                    {
                        for (int i = 0; i < contextMenu.Menu.Size(); i++)
                        {
                            if (!element.MenuItems[i].Text
                                    .Equals(contextMenu.Menu.GetItem(i)?.TitleFormatted?.ToString()))
                            {
                                DeconstructNativeMenu();
                                break;
                            }
                        }
                    }
                }

                return contextMenu;
            }

            private void ContextMenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
            {

                var item = ((ContextMenuContainer)Element).MenuItems.FirstOrDefault(x =>
                    x.Text == e.Item.TitleFormatted?.ToString());
                item?.OnItemTapped();
            }
#if MAUI
    }
#endif
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
