using System.Windows.Controls;

namespace ChildRegionsSample.Views
{
    public partial class Three : UserControl
    {
        public Three()
        {
            InitializeComponent();

            switch (SampleConfiguration.SampleMode)
            {
                case SampleMode.NoViewModel:
                    break;
                case SampleMode.Selectable:
                    this.DataContext = new ThreeAsSelectableViewModel();
                    break;
                case SampleMode.Singleton:
                    this.DataContext = new ThreedAsSingletonViewModel();
                    break;
                case SampleMode.ViewModel:
                    break;
                default:
                    break;
            }
        }
    }
}
