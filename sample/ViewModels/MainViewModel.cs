using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace APES.MAUI.Sample.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        #region Properties
        private string _text;
        public string Text
        {
            get => _text;
            set => SetField(ref _text, value);
        }
        
        private string _dynamicSectionText = "Dynamic section example - right-click items to test";
        public string DynamicSectionText
        {
            get => _dynamicSectionText;
            set => SetField(ref _dynamicSectionText, value);
        }
        #endregion

        #region Commands
        public ICommand FirstCommand { get; }
        public ICommand SecondCommand { get; }
        public ICommand DynamicSectionCommand { get; } // New command specifically for dynamic section
        public ICommand DestructiveCommand { get; }
        public ICommand ConstructiveCommand { get; }
        public ICommand NeverEndingCommand { get; }
        public ICommand ToggleConditionalCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand ClearItemsCommand { get; }
        #endregion

        #region Context Menu Properties
        private ContextMenuItems _imageContextItems = new();
        public ContextMenuItems ImageContextItems
        {
            get => _imageContextItems;
            set => SetField(ref _imageContextItems, value);
        }

        private bool _isConditionalActionEnabled = true;
        public bool IsConditionalActionEnabled
        {
            get => _isConditionalActionEnabled;
            set => SetField(ref _isConditionalActionEnabled, value);
        }

        private string _conditionalActionStatus = "Conditional action is currently enabled";
        public string ConditionalActionStatus
        {
            get => _conditionalActionStatus;
            set => SetField(ref _conditionalActionStatus, value);
        }

        private ObservableCollection<string> _listItems = new();
        public ObservableCollection<string> ListItems
        {
            get => _listItems;
            set => SetField(ref _listItems, value);
        }
        #endregion

        public MainViewModel()
        {
            Text = "Welcome to MAUI!\nHold or right-click to see context menu";
            
            // Initialize commands
            FirstCommand = new Command<string>((s) => OnFirstCommandExecuted(s));
            SecondCommand = new Command(OnSecondCommandExecuted);
            DynamicSectionCommand = new Command(OnDynamicSectionCommandExecuted);
            DestructiveCommand = new Command(DestructiveHandler);
            ConstructiveCommand = new Command(ConstructiveHandler);
            NeverEndingCommand = new AsyncCommand(NeverendingTask);
            ToggleConditionalCommand = new Command(ToggleConditionalAction);
            AddItemCommand = new Command(AddItem);
            ClearItemsCommand = new Command(ClearItems);
            _deleteIconSource = "outline_delete_black_24.png";
            SettingsIconSource = "outline_settings_black_24.png";
            
            // Initial setup
            FillAllImageActions();
            InitializeListItems();
        }

        private void OnFirstCommandExecuted(string s) => Text = s;

        private void OnSecondCommandExecuted() => Text = $"I was pressed at {DateTime.Now:HH:mm:ss}";
        
        private void OnDynamicSectionCommandExecuted() => DynamicSectionText = $"Dynamic section item clicked at {DateTime.Now:HH:mm:ss}";

        internal int SecondCounter { get; set; } = 0;

        private void DestructiveHandler()
        {
            ImageContextItems.Clear();
            ImageContextItems.Add(new ContextMenuItem()
            {
                Text = "Give me my actions back!",
                Command = ConstructiveCommand,
                Icon = SettingsIconSource,
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
            ImageContextItems.Clear();
            for (var i = 1; i < 5; i++)
            {
                ImageContextItems.Add(new ContextMenuItem()
                {
                    Text = $"Press me {i}!",
                    Command = DynamicSectionCommand,
                    Icon = i == 1 ? null : SettingsIconSource,
                    IsEnabled = i != 3,
                });
            }
            ImageContextItems.Add(new ContextMenuItem()
            {
                Text = "Remove context actions",
                Command = DestructiveCommand,
                IsDestructive = true,
                Icon = _deleteIconSource,
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

        private void ToggleConditionalAction()
        {
            IsConditionalActionEnabled = !IsConditionalActionEnabled;
            ConditionalActionStatus = IsConditionalActionEnabled 
                ? "Conditional action is currently enabled" 
                : "Conditional action is currently disabled";
        }

        private void InitializeListItems()
        {
            // Add some initial items
            ListItems.Add("Example Item 1");
            ListItems.Add("Example Item 2");
        }

        private void AddItem() => ListItems.Add($"New Item {ListItems.Count + 1}");

        private void ClearItems() => ListItems.Clear();

        private long _neverEndingCounter;
        public long NeverEndingCounter
        {
            get => _neverEndingCounter;
            set => SetField(ref _neverEndingCounter, value);
        }
        
        public FileImageSource SettingsIconSource { get; private set; }
        private readonly FileImageSource _deleteIconSource;
    }
}
