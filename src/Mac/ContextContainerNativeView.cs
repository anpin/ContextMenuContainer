using Foundation;
using AppKit;
using CoreText;
using CoreGraphics;
using Xamarin.Forms.Platform.MacOS;
using Xamarin.Forms.PlatformConfiguration.macOSSpecific;
using System;
namespace APES.UI.XF.Mac
{
    public class ContextContainerNativeView : NSView
    {
		readonly ContextMenuItems? MenuItems;
		NSMenu contextMenu;
		IVisualElementRenderer ChildRenderer;
        public ContextContainerNativeView(IVisualElementRenderer childRenderer, ContextMenuItems? contextMenuItems)
        {
			ChildRenderer = childRenderer ?? throw new ArgumentNullException(nameof(childRenderer));
			MenuItems = contextMenuItems;
            if(MenuItems != null)
				MenuItems.CollectionChanged += MenuItems_CollectionChanged;
			AddSubview(ChildRenderer.NativeView);
        }

        private void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
			RefillMenuItems();
        }

        public override void RightMouseDown(NSEvent theEvent)
		{
			HandleContextActions(theEvent);

			base.RightMouseDown(theEvent);
		}

		NSMenu? GetContextMenu()
		{
			if (contextMenu != null && MenuItems != null)
			{
				if (MenuItems.Count != contextMenu.Count)
				{
					RefillMenuItems();
				}
				else
				{
					for (int i = 0; i < contextMenu.Count; i++)
					{
						var nativeItem = contextMenu.ItemWithTag(i);
						if (!MenuItems[i].Text.Equals(nativeItem?.AttributedTitle.Value))
						{
							RefillMenuItems();
							break;
						}
					}
				}
			}
			return contextMenu;
		}
		void HandleContextActions(NSEvent theEvent)
		{
			if (MenuItems == null)
				return;
			var count = MenuItems.Count;
			if (count == 0)
				return;
			if (GetContextMenu() == null)
			{
				ConstructNativeMenu();
				FillMenuItems();

			}
			NSMenu.PopUpContextMenu(contextMenu, theEvent, this);
			
		}

		void ConstructNativeMenu()
        {
			contextMenu = new NSMenu();
        }
		

		void FillMenuItems()
		{
			if (MenuItems?.Count > 0)
			{
				for (var i = 0; i <  MenuItems.Count; i++)
				{
					contextMenu.AddItem(ToNSMenuItem(i, MenuItems[i]));
				}
				contextMenu.Update();
			}
		}
		void RefillMenuItems()
		{
			if (contextMenu == null)
				return;
			contextMenu.CancelTracking();
			contextMenu.RemoveAllItems();
			FillMenuItems();
		}
		public NSMenuItem ToNSMenuItem(int i, ContextMenuItem menuItem)
		{
			NSMenuItem nsMenuItem = new NSMenuItem();
			nsMenuItem.AttributedTitle = new NSAttributedString(menuItem.Text ?? "", foregroundColor: menuItem.IsDestructive ? NSColor.Red : null );
			nsMenuItem.Tag	 = i;
			nsMenuItem.Enabled = menuItem.IsEnabled;
			nsMenuItem.Activated += NsMenuItem_Activated;
			nsMenuItem.ValidateMenuItem = new Func<NSMenuItem, bool>((t) => t.Enabled);
			var nativeIcon = menuItem.Icon.ToNative();
			if (nativeIcon != null)
			{
				var elementColor = menuItem.IsDestructive ? NSColor.Red : NSAppearance.CurrentAppearance.Name == NSAppearance.NameDarkAqua ? NSColor.White : NSColor.Black;
				nsMenuItem.Image = ImageHandler.ImageTintedWithColor(nativeIcon, elementColor, new CGSize(25, 25));
			}
			return nsMenuItem;
		}

        private void NsMenuItem_Activated(object sender, EventArgs e)
        {
			var nsMenuItem = sender as NSMenuItem;
			if(nsMenuItem == null)
            {
				Logger.Error("Couldn't cast sender to NSMenuItem");
				return;
            }
			//that seems to have no effect 
			if (nsMenuItem.Enabled)
			{
				MenuItems[(int)nsMenuItem.Tag].InvokeCommand();
			}
		}
	}
}
