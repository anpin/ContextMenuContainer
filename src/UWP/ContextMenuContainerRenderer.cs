using System;
using System.Linq;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
#if MAUI
using Microsoft.Maui;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using WColors = Microsoft.UI.Colors;
using WBinding = Microsoft.UI.Xaml.Data.Binding;
using MenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using Setter = Microsoft.UI.Xaml.Setter;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;
using Style = Microsoft.UI.Xaml.Style;
using APES.UI.XF.UWP;

namespace APES.UI.XF;
[Preserve(AllMembers = true)]
sealed partial class ContextMenuContainerRenderer : ContentViewHandler
{
    private ContextMenuContainer Element => (ContextMenuContainer)VirtualView;

    protected override ContentPanel CreatePlatformView()
    {
        var result =base.CreatePlatformView();
        result.PointerReleased += PlatformViewPointerReleased;
        return result;
    }

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        
    }
#else
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;
using WColors = Windows.UI.Colors;
using WBinding = Windows.UI.Xaml.Data.Binding;
using APES.UI.XF;
using APES.UI.XF.UWP;

[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
namespace APES.UI.XF.UWP

{
    [Preserve(AllMembers = true)]
    public class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, ContentControl>
    {

        FrameworkElement? PlatformView;
        public ContextMenuContainerRenderer()
        {
          AutoPackage = false;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                //unsubscribe from events here
            }
            if (e.NewElement == null)
            {
                return;
            }
            if (Control == null)
            {
                SetNativeControl(new ContentControl());
            }
            Pack();
        }
     void Pack()
        {
            if (Element.Content == null)
            {
                return;
            }

            IVisualElementRenderer renderer = Element.Content.GetOrCreateRenderer();
            PlatformView = renderer.ContainerElement;
            PlatformView.PointerReleased += PlatformViewPointerReleased;
            //PlatformView.Holding += FrameworkElement_Holding;
            Control.Content = PlatformView;
        }
#endif
       

        private void PlatformViewPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(PlatformView);
            if (point.Properties.PointerUpdateKind != PointerUpdateKind.RightButtonReleased)
                return;
            try
            {
                if (Element.HasMenuOptions())
                    OpenContextMenu();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        MenuFlyout? GetContextMenu()
        {
            if (FlyoutBase.GetAttachedFlyout(PlatformView) is MenuFlyout flyout)
            {
                var actions = Element.MenuItems;
                if (flyout.Items.Count != actions.Count)
                    return null;

                for (int i = 0; i < flyout.Items.Count; i++)
                {
                    if (flyout.Items[i].DataContext != actions[i])
                        return null;
                }
                return flyout;
            }
            return null;
        }
        void OpenContextMenu()
        {
            if (GetContextMenu() == null)
            {
                var flyout = new MenuFlyout();
                SetupMenuItems(flyout);

                Element.MenuItems.CollectionChanged += MenuItems_CollectionChanged;

                FlyoutBase.SetAttachedFlyout(PlatformView, flyout);
            }

            FlyoutBase.ShowAttachedFlyout(PlatformView);
        }

        private void MenuItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var menu = GetContextMenu();
            if (menu != null)
            {
                menu.Items.Clear();
                SetupMenuItems(menu);
            }
        }

        void SetupMenuItems(MenuFlyout menu)
        {

            foreach (var item in Element.MenuItems)
            {
                AddMenuItem(menu, item);
            }
        }
        void AddMenuItem(MenuFlyout contextMenu, ContextMenuItem item)
        {
            var nativeItem = new MenuFlyoutItem();
            nativeItem.SetBinding(MenuFlyoutItem.TextProperty, new WBinding()
            {
                Path = new PropertyPath(nameof(ContextMenuItem.Text)),
            });

            //nativeItem.SetBinding(MenuFlyoutItem.CommandProperty, new WBinding()
            //{
            //    Path = new PropertyPath(nameof(ContextMenuItem.Command)),
            //});

            //nativeItem.SetBinding(MenuFlyoutItem.CommandParameterProperty, new WBinding()
            //{
            //    Path = new PropertyPath(nameof(ContextMenuItem.CommandParameter)),
            //});

            nativeItem.SetBinding(MenuFlyoutItem.IconProperty, new WBinding()
            {
                Path = new PropertyPath(nameof(ContextMenuItem.Icon)),
                Converter = ImageConverter,
            });
            nativeItem.SetBinding(MenuFlyoutItem.StyleProperty, new WBinding()
            {
                Path = new PropertyPath(nameof(ContextMenuItem.IsDestructive)),
                Converter = BoolToStytleConverter,
            });
            nativeItem.SetBinding(MenuFlyoutItem.IsEnabledProperty, new WBinding()
            {
                Path = new PropertyPath(nameof(ContextMenuItem.IsEnabled)),
            });
            nativeItem.Click += NativeItem_Click;
            nativeItem.DataContext = item;
            contextMenu.Items.Add(nativeItem);
        }

        private void NativeItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            if(item == null)
            {
                Logger.Error("Couldn't cast to MenuFlyoutItem");
                return;
            }
            var context = item.DataContext as ContextMenuItem;
            if (context == null)
            {
                Logger.Error("Couldn't cast MenuFlyoutItem.DataContext to ContextMenuItem");
                return;
            }
            context.OnItemTapped();
        }

        static Style DestructiveStyle { get; } = new Style()
        {
            TargetType = typeof(MenuFlyoutItem),
            Setters =
            {
                new Setter(MenuFlyoutItem.ForegroundProperty, new SolidColorBrush(WColors.Red)),
            }
        };
        static Style NondDestructiveStyle { get; } = new Style()
        {
            TargetType = typeof(MenuFlyoutItem),
            Setters =
            {
                //new Setter(MenuFlyoutItem.ForegroundProperty, new SolidColorBrush(WColors.Red)),
            }
        };
        static FileImageSourceToBitmapIconSourceConverter ImageConverter { get; } = new FileImageSourceToBitmapIconSourceConverter();
        static GenericBoolConverter<Style> BoolToStytleConverter { get; } = new(DestructiveStyle, NondDestructiveStyle);
    }
#if !MAUI 
}
#endif
