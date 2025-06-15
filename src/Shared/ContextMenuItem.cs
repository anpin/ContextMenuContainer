// MIT License
// Copyright (c) 2021 Pavel Anpin

using System.Windows.Input;

using Microsoft.Maui.Controls;

namespace APES.MAUI;

public delegate void ItemTapped(ContextMenuItem item);

public class ContextMenuItem : Element
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ContextMenuItem));

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ContextMenuItem));

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ContextMenuItem));

    public static readonly BindableProperty IsEnabledProperty = BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(ContextMenuItem), true);

    public static readonly BindableProperty IsDestructiveProperty = BindableProperty.Create(nameof(IsDestructive), typeof(bool), typeof(ContextMenuItem));

    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(FileImageSource), typeof(ContextMenuItem));

    public event ItemTapped? ItemTapped;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    public bool IsDestructive
    {
        get => (bool)GetValue(IsDestructiveProperty);
        set => SetValue(IsDestructiveProperty, value);
    }

    public FileImageSource? Icon
    {
        get => (FileImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    internal void OnItemTapped()
    {
        ItemTapped?.Invoke(this);
        if (Command?.CanExecute(CommandParameter) ?? IsEnabled)
        {
            Command?.Execute(CommandParameter);
        }
    }
}
