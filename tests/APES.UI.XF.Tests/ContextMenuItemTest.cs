using System;
using System.Collections.Generic;
using Xunit;
using APES.UI.XF;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
namespace APES.UI.XF.Tests
{
    public class ContextMenuItemTest
    {
        public ContextMenuItemTest()
        {
            Xamarin.Forms.Mocks.MockForms.Init();
        }
        class vm : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            protected bool SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                NotifyPropertyChanged(propertyName);
                return true;
            }
            string text;
            public string Text
            {
                get => text;
                set => SetField(ref text, value);
            }
            string cmdParam;
            public string CmdParam
            {
                get => cmdParam;
                set => SetField(ref cmdParam, value);
            }
            bool enabled;
            public bool Enabled
            {
                get => enabled;
                set => SetField(ref enabled, value);
            }
            ImageSource icon;
            public ImageSource Icon
            {
                get => icon;
                set => SetField(ref icon, value);
            }
            ICommand cmd;
            public ICommand Cmd
            {
                get => cmd;
                set => SetField(ref cmd, value);
            }
            public vm()
            {
                Text = Guid.NewGuid().ToString();
                Cmd = new Command<string>((p) => Text = p);
                CmdParam = Guid.NewGuid().ToString();
                Enabled = true;
                Icon = ImageSource.FromFile("logo.png");
            }

        }

        [Fact]
        public void CreateAndRun()
        {
            var vm = new vm();
            var item = new ContextMenuItem();
            var eventFired = false;
            var eventHadArg = false;
            Assert.NotNull(item);
            item.BindingContext = vm;
            item.SetBinding(ContextMenuItem.TextProperty, nameof(vm.Text));
            item.SetBinding(ContextMenuItem.CommandProperty, nameof(vm.Cmd));
            item.SetBinding(ContextMenuItem.CommandParameterProperty, nameof(vm.CmdParam));
            item.SetBinding(ContextMenuItem.IsEnabledProperty, nameof(vm.Enabled));
            item.SetBinding(ContextMenuItem.IconProperty, nameof(vm.Icon));
            item.ItemTapped += (s, e) =>
            {
                eventFired = true;
                var item = s as ContextMenuItem;
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                eventHadArg = item?.CommandParameter == vm.CmdParam;
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            };
            Assert.Equal(item.Text, vm.Text);
            Assert.Equal(item.Command, vm.Cmd);
            Assert.Equal(item.CommandParameter, vm.CmdParam);
            Assert.Equal(item.IsEnabled, vm.Enabled);
            Assert.Equal(item.Icon, vm.Icon);
            item.OnItemTapped();
            Assert.Equal(item.Text, vm.CmdParam);
            Assert.True(eventFired);
            Assert.True(eventHadArg);
        }
    }
}
