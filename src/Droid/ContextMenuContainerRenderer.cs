using System;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Text;
using Android.Text.Style;
using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;
using DrawableWrapperX = AndroidX.AppCompat.Graphics.Drawable.DrawableWrapper;
using Java.Lang.Reflect;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using APES.UI.XF;
using APES.UI.XF.Droid;
using Path = System.IO.Path;
using AColor = Android.Graphics.Color;
[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
namespace APES.UI.XF.Droid
{
    public class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, Android.Views.View>
    {
        IVisualElementRenderer? childRenderer;
        PopupMenu? contextMenu;
        //ViewCellRenderer? _listViewparent;
        public ContextMenuContainerRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement is ContextMenuContainer old)
            {
                old.BindingContextChanged -= Element_BindingContextChanged;
                old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;
            }
            if (e.NewElement == null || e.NewElement.Content == null)
            {
                return;
            }
            childRenderer = Platform.CreateRendererWithContext(Element.Content, Context);
            this.childRenderer.View.Background = null;
            SetNativeControl(childRenderer.View);
        }
        void ConstructNativeMenu()
        {
            if (childRenderer == null)
                return;
            contextMenu = new PopupMenu(Context, childRenderer.View);
            contextMenu.MenuItemClick += ContextMenu_MenuItemClick;
            Element.BindingContextChanged += Element_BindingContextChanged;
            Element.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
            Field field = contextMenu.Class.GetDeclaredField("mPopup");
            field.Accessible = true;
            Java.Lang.Object? menuPopupHelper = field.Get(contextMenu);
            Method? setForceIcons = menuPopupHelper?.Class.GetDeclaredMethod("setForceShowIcon", Java.Lang.Boolean.Type);
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
                    Drawable? drawable = (int)Build.VERSION.SdkInt >= 21 ?
                                                 Context?.GetDrawable(id) :
                                                 Context?.GetDrawable(name);
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
            if (Element != null && Element.MenuItems?.Count > 0)
            {
                foreach (var item in Element.MenuItems)
                {
                    AddMenuItem(item);
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
            if (contextMenu != null)
            {
                if (Element.MenuItems.Count != contextMenu.Menu.Size())
                {
                    DeconstructNativeMenu();
                }
                else
                {
                    for (int i = 0; i < contextMenu.Menu.Size(); i++)
                    {
                        if (!Element.MenuItems[i].Text.Equals(contextMenu.Menu.GetItem(i)?.TitleFormatted?.ToString()))
                        {
                            DeconstructNativeMenu();
                            break;
                        }
                    }
                }
            }
            return contextMenu;
        }
        private void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefillMenuItems();
        }

        private void Element_BindingContextChanged(object sender, EventArgs e)
        {
            RefillMenuItems();
        }
        private void ContextMenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            var item = Element.MenuItems.FirstOrDefault(x => x.Text == e.Item.TitleFormatted?.ToString());
            item?.InvokeCommand();
        }
        bool enabled => Element.MenuItems.Count > 0;
        MyTimer timer;
        bool timerFired = false;

        public override bool OnTouchEvent(MotionEvent ev)
        {
            Logger.Debug("ContextMEnuContainer DispatchToucEvent fired {0}", ev.Action);
            if (enabled && ev.Action == MotionEventActions.Down)
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
                return true;
            }
            else if (ev.Action == MotionEventActions.Up || ev.Action == MotionEventActions.Cancel)
            {
                timer?.Stop();
                return base.OnTouchEvent(ev);
            }
            else
            {
                return base.OnTouchEvent(ev);
            }
        }
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {

            if (enabled)
            {
                switch (ev.Action)
                {
                    case MotionEventActions.Down:
                    case MotionEventActions.Up:
                    case MotionEventActions.Cancel:
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return base.OnInterceptTouchEvent(ev);
            }
        }

        //TouchListner? touchListner;
        //TouchListner TouchListner
        //{
        //    get
        //    {
        //        if (touchListner == null)
        //        {
        //            touchListner = new TouchListner(HoldGestureDetector);
        //        }
        //        return touchListner;
        //    }
        //}
        //GestureDetector? holdGestureDetector;
        //GestureDetector HoldGestureDetector
        //{
        //    get
        //    {
        //        if (holdGestureDetector == null)
        //        {
        //            holdGestureDetector = new GestureDetector(Context, new HoldGestureRecognizer(OpenContextMenu));
        //        }
        //        return holdGestureDetector;
        //    }
        //}
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
