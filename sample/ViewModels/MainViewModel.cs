using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace APES.MAUI.Sample.ViewModels
{


    public abstract class ViewModelBase : INotifyPropertyChanged
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
    }

     /// <summary>
    /// Represents a person for the CollectionView example.
    /// </summary>
    public class Person : ViewModelBase
    {
        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set
            {
                SetField(ref _firstName, value);
                NotifyPropertyChanged(nameof(FullName));
                NotifyPropertyChanged(nameof(FirstLetter));
                NotifyPropertyChanged(nameof(AutomationId));
                NotifyPropertyChanged(nameof(NameAutomationId));
            }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set
            {
                SetField(ref _lastName, value);
                NotifyPropertyChanged(nameof(FullName));
                NotifyPropertyChanged(nameof(AutomationId));
                NotifyPropertyChanged(nameof(NameAutomationId));
            }
        }

        public string FullName => $"{FirstName} {LastName}";
        public string FirstLetter => string.IsNullOrEmpty(FirstName) ? "#" : FirstName[0].ToString().ToUpper();
        public string AutomationId => $"person_{FirstName?.ToLower()}_{LastName?.ToLower()}";
        public string NameAutomationId => $"person_name_{FirstName?.ToLower()}_{LastName?.ToLower()}";
    }

    public interface IMainViewModel {
        public ICommand FirstCommand { get; }
        public ICommand SecondCommand { get; }
        public ICommand DynamicSectionCommand { get; }
        public ICommand DestructiveCommand { get; }
        public ICommand ConstructiveCommand { get; }
        public ICommand NeverEndingCommand { get; }
        public ICommand ToggleConditionalCommand { get; }
        public ICommand ConditionalCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand ClearItemsCommand { get; }
        public ICommand EditPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }
    }
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
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
        public ICommand ConditionalCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand ClearItemsCommand { get; }
        public ICommand EditPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }
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


        private string _collectionViewResultText = "Right-click on a person to see actions";
        public string CollectionViewResultText
        {
            get => _collectionViewResultText;
            set => SetField(ref _collectionViewResultText, value);
        }

        private ObservableCollection<Person> _listItems = new();
        public ObservableCollection<Person> ListItems
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
            ConditionalCommand = new Command(ConditionalAction);
            AddItemCommand = new Command(AddItem);
            ClearItemsCommand = new Command(ClearItems);
            EditPersonCommand = new Command<Person>(EditPerson);
            DeletePersonCommand = new Command<Person>(DeletePerson);
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
            // ReSharper disable once FunctionNeverReturns
        }

        private void ToggleConditionalAction()
        {
            IsConditionalActionEnabled = !IsConditionalActionEnabled;
            ConditionalActionStatus = IsConditionalActionEnabled 
                ? "Conditional action is currently enabled" 
                : "Conditional action is currently disabled";
        }
        private void ConditionalAction() => ConditionalCommandCounter++;

        private void InitializeListItems()
        {
            // Add some initial items
            // ListItems.Add(new Person { FirstName = "Thiago", LastName = "Ferreira" });
            // ListItems.Add(new Person { FirstName = "Noor", LastName = "Abdallah" });
            // ListItems.Add(new Person { FirstName = "Mateo", LastName = "Garcia" });
            // ListItems.Add(new Person { FirstName = "Yuki", LastName = "Tanaka" });
            AddItem();
            AddItem();
            
        }

        private readonly List<(string First, string Last)> _namePool = new() {
            ("Dmitri", "Ivanov"),       
            ("João", "Silva"),          
            ("Priya", "Sharma"),        
            ("Ahmed", "Al-Sayed"),      
            ("Anastasia", "Volkova"),   
            ("Pedro", "Oliveira"),      
            ("Rahul", "Verma"),         
            ("Fatima", "Hassan"),       
            ("Ivan", "Sokolov"),        
            ("Ana", "Costa"),           
            ("Ananya", "Singh"),        
            ("Omar", "Khalid"),         
            ("Olga", "Smirnova"),       
            ("Maria", "Santos"),        
            ("Rohan", "Gupta"),         
            ("Layla", "Ibrahim"),       
            ("Vladimir", "Popov"),      
            ("Lucas", "Pereira"),       
            ("Vikram", "Patel"),        
            ("Youssef", "Mahmoud")      
        };

        private void AddItem() { 
            var c = ListItems.Count;
            (string First, string Last) p;
            if(c < _namePool.Count) {
                p = _namePool[ListItems.Count];
            } else
            {
                p = ("New person", $"#{c}");
            }
            ListItems.Add(new Person() { FirstName = p.First, LastName = p.Last });
        }

        private void ClearItems() => ListItems.Clear();

        private long _neverEndingCounter;
        public long NeverEndingCounter
        {
            get => _neverEndingCounter;
            set => SetField(ref _neverEndingCounter, value);
        }
        
        private long _conditionalCommandCounter;
        public long ConditionalCommandCounter
        {
            get => _conditionalCommandCounter;
            set => SetField(ref _conditionalCommandCounter, value);
        }

        public FileImageSource SettingsIconSource { get; private set; }
        private readonly FileImageSource _deleteIconSource;

        private void EditPerson(Person person)
        {
            if (person == null){
                CollectionViewResultText = "Person was null!";
                Logger.Error("ContextMenuContainer EditPerson person was null!");
                return;
            } 
            CollectionViewResultText = $"Editing: {person. FullName} at {DateTime.Now:HH: mm:ss}";
        }

        private void DeletePerson(Person person)
        {
            

            try 
            {
            if (person == null){
                CollectionViewResultText = "Person was null!";
                Logger.Error("ContextMenuContainer DeletePerson person was null!");
                return;
            } 
            var index = ListItems.IndexOf(person);
            if (index < 0 )
            {
                CollectionViewResultText = "Person was not found!";
                Logger.Error("ContextMenuContainer DeletePerson person return index less than 0!");
            }
            ListItems.RemoveAt(index);
            CollectionViewResultText = $"Deleted: {person.FullName} at {DateTime.Now:HH:mm:ss}";
            }
            catch(Exception ex)
            {
                    CollectionViewResultText = ex.Message;

            }
        }


    }
}
