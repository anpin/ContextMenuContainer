using System.ComponentModel;
using System.Windows.Input;
#if MAUI
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#else
using Xamarin.Forms;
#endif
namespace APES.UI.XF.Sample.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        private string text = "Default text";
        public string Text
        {
            get => text;
            set => SetField(ref text, value);
        }
        public ICommand FirstCommand { get; }
        public ICommand SecondCommand { get; }

        public ICommand DestructiveCommand { get; }

        public ICommand ConstructiveCommand { get; }
        public ICommand NeverEndingCommand { get; }
        private ContextMenuItems imageContextItems = new ContextMenuItems();
        public ContextMenuItems ImageContextItems
        {
            get => imageContextItems;
            set => SetField(ref imageContextItems, value);
        }

        public MainViewModel()
        {
            Text = "Welcome to Xamarin.Forms!\nHold or right-click to see context menu";
            FirstCommand = new Command<string>((s) => OnFirstCommandExecuted(s));
            SecondCommand = new Command(OnSecondCommandExecuted);
            DestructiveCommand = new Command(DestructiveHandler);
            ConstructiveCommand = new Command(ConstructiveHandler);
            NeverEndingCommand = new AsyncCommand(NeverendingTask);
#if !MAUI
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    logoIconSource = "logo.png";
                    deleteIconSource = "outline_delete_24.xml";
                    settingsIconSource = "outline_settings_black_24.png";
                    break;
                case Device.iOS:
                case Device.macOS:
                    logoIconSource = "logo.png";
                    deleteIconSource = "outline_delete_black_24.png";
                    settingsIconSource = "outline_settings_black_24.png";
                    break;
                case Device.UWP:
                    logoIconSource = @"Assets\logo.png";
                    deleteIconSource = @"Assets\outline_delete_black_24.png";
                    settingsIconSource = @"Assets\outline_settings_black_24.png";
                    break;
            }
#else
            //
            // {
            //     logoIconSource = "logo.png";
            //     deleteIconSource = "outline_delete_24.xml";
            //     settingsIconSource = "outline_settings_black_24.png";
            // }
            // else if (DeviceInfo.Platform == DevicePlatform.iOS || 
            //          DeviceInfo.Platform == DevicePlatform.macOS ||
            //          DeviceInfo.Platform == DevicePlatform.MacCatalyst)
            // {
            //
            //     logoIconSource = "logo.png";
            //     deleteIconSource = "outline_delete_black_24.png";
            //     settingsIconSource = "outline_settings_black_24.png";
            // }
            //
            // else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            // {
                logoIconSource = "logo.png";
                deleteIconSource = "outline_delete_black_24.png";
                settingsIconSource = "outline_settings_black_24.png";
            // }
#endif

            FillAllImageActions();
        }

        private void OnFirstCommandExecuted(string s) => Text = s;

        private void OnSecondCommandExecuted() => Text = $"Action was pressed {++SecondCounter} times!";
        internal int SecondCounter { get; set; } = 0;

        private void DestructiveHandler()
        {
            ImageContextItems.Clear();
            ImageContextItems.Add(new ContextMenuItem()
            {
                Text = "Give me my actions back!",
                Command = ConstructiveCommand,
                Icon = settingsIconSource,
            });
            NotifyPropertyChanged(nameof(ImageContextItems));

        }

        private void ConstructiveHandler()
        {
            ImageContextItems.Clear();
            FillAllImageActions();
        }

        private void FillAllImageActions()
        {
            for (var i = 1; i < 5; i++)
            {
                ImageContextItems.Add(new ContextMenuItem()
                {
                    Text = $"Press me {i}!",
                    Command = SecondCommand,
                    Icon = i == 1 ? null : settingsIconSource,
                    IsEnabled = i != 3,
                });
            }
            ImageContextItems.Add(new ContextMenuItem()
            {
                Text = "Remove context actions",
                Command = DestructiveCommand,
                IsDestructive = true,
                Icon = deleteIconSource,
            });
            NotifyPropertyChanged(nameof(ImageContextItems));
        }

        private async Task NeverendingTask()
        {
            while(true)
            {

                NeverEndingCounter++;
                await Task.Delay(5000);
            }
        }

        private long neverEndingCounter;
        public long NeverEndingCounter
        {
            get => neverEndingCounter;
            set => SetField(ref neverEndingCounter, value);
        }


        private readonly FileImageSource logoIconSource;
        public FileImageSource LogoIconSource => logoIconSource;
        private readonly FileImageSource deleteIconSource;
        private readonly FileImageSource settingsIconSource;
        public FileImageSource SettingsIconSource => settingsIconSource;

    }
}
