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
		ContextMenuItems? MenuItems;
		IVisualElementRenderer ChildRenderer;
        public ContextContainerNativeView(IVisualElementRenderer childRenderer, ContextMenuItems? contextMenuItems)
        {
			ChildRenderer = childRenderer ?? throw new ArgumentNullException(nameof(childRenderer));
			MenuItems = contextMenuItems;
			AddSubview(ChildRenderer.NativeView);
        }

		public override void RightMouseDown(NSEvent theEvent)
		{
			HandleContextActions(theEvent);

			base.RightMouseDown(theEvent);
		}

		void HandleContextActions(NSEvent theEvent)
		{
			if (MenuItems == null)
				return;
			var count = MenuItems.Count;
			if (count == 0)
				return;
			
				NSMenu menu = new NSMenu();
				for (int i = 0; i < count; i++)
				{
					menu.AddItem(ToNSMenuItem(i, MenuItems[i]));
				}

				NSMenu.PopUpContextMenu(menu, theEvent, this);
			
		}
		public NSMenuItem ToNSMenuItem(int i, ContextMenuItem menuItem)
		{
			NSMenuItem nsMenuItem = new NSMenuItem();
			nsMenuItem.AttributedTitle = new NSAttributedString(menuItem.Text ?? "", foregroundColor: menuItem.IsDestructive ? NSColor.Red : null );
			nsMenuItem.Tag = i;
			nsMenuItem.Enabled = menuItem.IsEnabled;	
			nsMenuItem.Activated += (sender, e) =>
			{
				menuItem.InvokeCommand();
			};
			var nativeIcon = menuItem.Icon.ToNative();
			if (nativeIcon != null)
			{
				var elementColor = menuItem.IsDestructive ? NSColor.Red : NSAppearance.CurrentAppearance.Name == NSAppearance.NameDarkAqua ? NSColor.White : NSColor.Black;
				nsMenuItem.Image = ImageHandler.ImageTintedWithColor(nativeIcon, elementColor, new CGSize(25, 25));
			}
			return nsMenuItem;
		}

	}
}
