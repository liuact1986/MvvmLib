using MvvmLib.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NavigationSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyMasterDetailPage : MasterDetailPage
    {
        public MyMasterDetailPage()
        {
            InitializeComponent();

            if (!NavigationManager.IsRegistered("MasterDetail"))
            {
                NavigationManager.Register(MasterDetail, "MasterDetail");
            }
        }
    }
}