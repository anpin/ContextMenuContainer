using System.Collections.ObjectModel;
namespace APES.UI.XF.UWP
{
    public static class Initialize
    {
        public static void Init()
        {
            ContextMenuContainer? c = new ContextMenuContainer();
            ContextMenuItem? i = new ContextMenuItem();
            ContextMenuContainerRenderer? r = new ContextMenuContainerRenderer();
            c = null;
            i = null;
            r = null;
        }
    }
}
