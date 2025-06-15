// MIT License
// Copyright (c) 2021 Pavel Anpin

using Microsoft.Maui.Controls;

namespace APES.MAUI.Sample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        ContextMenuContainer.Init();
        MainPage = new NavPage(new MainPage());
	}
}
