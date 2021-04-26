using AppKit;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;
namespace APES.UI.XF.Sample.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        NSWindow _window;
        public AppDelegate()
        {
            _window = NewWindow();
        }

        public override NSWindow MainWindow
        {
            get { return _window; }
        }
        public override void DidFinishLaunching(NSNotification notification)
        {
            NSApplication.SharedApplication.MainMenu = SetupMenuNative();
            Forms.Init();
            var app = new App();
            app.UserAppTheme = NSAppearance.CurrentAppearance.Name == NSAppearance.NameDarkAqua ? OSAppTheme.Dark : OSAppTheme.Light;
            LoadApplication(app);
            base.DidFinishLaunching(notification);

        }
        NSWindow NewWindow()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

            var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
            var window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            window.Title = "APES UI Sample";
            window.TitleVisibility = NSWindowTitleVisibility.Hidden;
            return window;
        }
        [Export("applicationShouldTerminateAfterLastWindowClosed")]
        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
        NSMenu SetupMenuNative()
        {
            // top bar app menu
            NSMenu menubar = new NSMenu();
            NSMenuItem appMenuItem = new NSMenuItem();
            menubar.AddItem(appMenuItem);

            NSMenu appMenu = new NSMenu();
            appMenuItem.Submenu = appMenu;

            //var newWindowtMenuItem = new NSMenuItem("New window", "n", delegate
            //{
            //    NewDocument(menubar);
            //});
            //appMenu.AddItem(newWindowtMenuItem);

            // add separator
            NSMenuItem separator = NSMenuItem.SeparatorItem;
            appMenu.AddItem(separator);

            // add quit menu item

            var quitMenuItem = new NSMenuItem("Quit APES UI Sample", "q", delegate
            {
                NSApplication.SharedApplication.Terminate(menubar);
            });
            appMenu.AddItem(quitMenuItem);

            return menubar;
        }
    }
}
