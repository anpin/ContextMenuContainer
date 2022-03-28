using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
#if MAUI
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#else
using Xamarin.Forms;
#endif
namespace APES.UI.XF
{
    public partial class ContextMenuItem : Element
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ContextMenuItem));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ContextMenuItem));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ContextMenuItem));
        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly BindableProperty IsEnabledProperty = BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(ContextMenuItem), true);
        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public static readonly BindableProperty IsDestructiveProperty = BindableProperty.Create(nameof(IsDestructive), typeof(bool), typeof(ContextMenuItem));
        public bool IsDestructive
        {
            get => (bool)GetValue(IsDestructiveProperty);
            set => SetValue(IsDestructiveProperty, value);
        }

        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(FileImageSource), typeof(ContextMenuItem));
        public FileImageSource Icon
        {
            get => (FileImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        internal void OnItemTapped()
        {

            ItemTapped?.Invoke(this, new EventArgs());
            if (Command?.CanExecute(CommandParameter) ?? false && IsEnabled)
            {
                Command.Execute(CommandParameter);
            }
        }
        public event EventHandler? ItemTapped;
    }
}
