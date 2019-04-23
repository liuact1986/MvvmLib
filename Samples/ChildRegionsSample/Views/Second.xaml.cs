using System.Windows.Controls;

namespace ChildRegionsSample.Views
{
    public partial class Second : UserControl
    {
        public Second()
        {
            InitializeComponent();

            switch (SampleConfiguration.SampleMode)
            {
                case SampleMode.NoViewModel:
                    break;
                case SampleMode.Selectable:
                    this.DataContext = new SecondAsSelectableViewModel();
                    break;
                case SampleMode.Singleton:
                    this.DataContext = new SecondAsSingletonViewModel();
                    break;
                case SampleMode.ViewModel:
                    break;
                default:
                    break;
            }
        }
    }
}
