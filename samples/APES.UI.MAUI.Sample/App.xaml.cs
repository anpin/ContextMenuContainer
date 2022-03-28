namespace APES.UI.MAUI.Sample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        APES.UI.XF.ContextMenuContainer.Init();
        MainPage = new NavPage(new MainPage());
	}
}
