using MvvmLib.Adaptive;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace AdaptiveSample.Windows
{

    public sealed partial class PageOne : Page
    {
        public PageOne()
        {
            this.InitializeComponent();

            // add bindings with json file 
            // and / or in code
            this.bp.
                AddBreakpointWithBindings(1260, AdaptiveContainer.Create().Set("margin", "200, 120").Set("titleFontSize", 48).Set("titleForeground", "brown").Get())
               .AddBreakpointWithBindings(1600, AdaptiveContainer.Create().Set("margin", "320, 160").Set("titleFontSize", 60).Set("titleForeground", "black").Get());

            this.SizeChanged += PageOne_SizeChanged;
            this.bp.ActiveChanged += AdaptiveBinding_ActiveChanged;
        }

        private void AdaptiveBinding_ActiveChanged(object sender, ActiveChangedEventArgs e)
        {
            // binding elements or in code
            // TextBlock.FontSize = (double) e.Active["titleFontSize"];
            // etc.
        }

        private void PageOne_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeTextBlock.Text = e.NewSize.Width.ToString();
        }

    }
}
