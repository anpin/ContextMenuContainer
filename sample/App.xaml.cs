// MIT License
// Copyright (c) 2021 Pavel Anpin

using Microsoft.Maui.Controls;

namespace APES.MAUI.Sample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}
	
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window( new NavPage(new MainPage()));
    }
}
