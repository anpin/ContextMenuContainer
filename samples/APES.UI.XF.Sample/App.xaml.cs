using Xamarin.Forms;

namespace APES.UI.XF.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ContextMenuContainer.Init();
            MainPage = new NavPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
