using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APES.UI.XF.Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavPage : NavigationPage
    {
        public NavPage(Page root) : base(root)
        {
            InitializeComponent();
        }
    }
}