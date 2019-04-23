using System.Windows.Controls;

namespace ChildRegionsSample.Views
{

    public partial class First : UserControl
    {
        public First()
        {
            InitializeComponent();

            switch (SampleConfiguration.SampleMode)
            {
                case SampleMode.NoViewModel:
                    break;
                case SampleMode.Selectable:
                    this.DataContext = new FirstChildAsSelectableViewModel();
                    break;
                case SampleMode.Singleton:
                    this.DataContext = new FirstAsSingletonViewModel();
                    break;
                case SampleMode.ViewModel:
                    break;
                default:
                    break;
            } 
        }
    }
}
