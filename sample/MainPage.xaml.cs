using Microsoft.Maui.Controls;
using  APES.MAUI.Sample.ViewModels;
namespace APES.MAUI.Sample;
public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public IMainViewModel ViewModel => BindingContext as IMainViewModel;

    }

