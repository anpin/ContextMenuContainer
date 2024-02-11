// MIT License
// Copyright (c) 2021 Pavel Anpin

namespace APES.UI.MAUI.Sample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        XF.ContextMenuContainer.Init();
        MainPage = new NavPage(new MainPage());
	}
}
