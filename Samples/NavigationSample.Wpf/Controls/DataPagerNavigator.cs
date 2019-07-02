using MvvmLib.Navigation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace NavigationSample.Wpf.Controls
{
    public class DataPagerNavigator : Control
    {
        private Button moveToFirstPageButton;
        private Button moveToPreviousPageButton;
        private Button moveToNextPageButton;
        private Button moveToLastPageButton;
        private TextBox currentPageTextBox;
        private ItemsControl numericButtons;
        private string groupName;

        public DataPager DataPager
        {
            get { return (DataPager)GetValue(DataPagerProperty); }
            set { SetValue(DataPagerProperty, value); }
        }

        public static readonly DependencyProperty DataPagerProperty =
            DependencyProperty.Register("DataPager", typeof(DataPager), typeof(DataPagerNavigator), new PropertyMetadata(null));

        public int NumericButtonCount
        {
            get { return (int)GetValue(NumericButtonCountProperty); }
            set { SetValue(NumericButtonCountProperty, value); }
        }

        public static readonly DependencyProperty NumericButtonCountProperty =
            DependencyProperty.Register("NumericButtonCount", typeof(int), typeof(DataPagerNavigator), new PropertyMetadata(0));

        public AutoEllipsisMode AutoEllipsisMode
        {
            get { return (AutoEllipsisMode)GetValue(AutoEllipsisModeProperty); }
            set { SetValue(AutoEllipsisModeProperty, value); }
        }

        public static readonly DependencyProperty AutoEllipsisModeProperty =
            DependencyProperty.Register("AutoEllipsisMode", typeof(AutoEllipsisMode), typeof(DataPagerNavigator), new PropertyMetadata(AutoEllipsisMode.None));


        static DataPagerNavigator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataPagerNavigator), new FrameworkPropertyMetadata(typeof(DataPagerNavigator)));
        }

        public DataPagerNavigator()
        {
            this.groupName = Guid.NewGuid().ToString();
            this.Loaded += OnLoaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.moveToFirstPageButton = this.GetTemplateChild("MoveToFirstPageButton") as Button;
            this.moveToPreviousPageButton = GetTemplateChild("MoveToPreviousPageButton") as Button;
            this.moveToNextPageButton = GetTemplateChild("MoveToNextPageButton") as Button;
            this.moveToLastPageButton = GetTemplateChild("MoveToLastPageButton") as Button;
            this.currentPageTextBox = GetTemplateChild("CurrentPageTextBox") as TextBox;
            this.numericButtons = GetTemplateChild("NumericButtons") as ItemsControl;
        }

        private bool IsInDesignMode(DependencyObject element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }

        private void HandleEvents()
        {
            this.moveToFirstPageButton.Click -= OnMoveToFirstPageButtonClick;
            this.moveToPreviousPageButton.Click -= OnMoveToPreviousPageButtonClick;
            this.moveToNextPageButton.Click -= OnMoveToNextPageButtonClick;
            this.moveToLastPageButton.Click -= OnMoveToLastPageButtonClick;
            this.currentPageTextBox.KeyUp -= OnCurrentPageTextBoxKeyUp;

            this.moveToFirstPageButton.Click += OnMoveToFirstPageButtonClick;
            this.moveToPreviousPageButton.Click += OnMoveToPreviousPageButtonClick;
            this.moveToNextPageButton.Click += OnMoveToNextPageButtonClick;
            this.moveToLastPageButton.Click += OnMoveToLastPageButtonClick;
            this.currentPageTextBox.KeyUp += OnCurrentPageTextBoxKeyUp;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!IsInDesignMode(this))
            {
                if (this.DataPager == null)
                    throw new ArgumentNullException(nameof(DataPager));

                HandleEvents();

                Refresh();

                this.DataPager.Refreshed += OnDataPagerRefreshed;
            }
        }


        private void OnDataPagerRefreshed(object sender, EventArgs e)
        {
            Refresh();
        }

        private void SelectPageAndUpdateVisualStates()
        {
            if (AutoEllipsisMode == AutoEllipsisMode.Both)
                SelectNumericButton();
            UpdateVisualStates();
        }

        public void Refresh()
        {
            if (AutoEllipsisMode == AutoEllipsisMode.Both)
                CreateNumericButtons(0);

            UpdateVisualStates();
        }

        private void OnCurrentPageTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (int.TryParse(textBox.Text, out int currentPage))
                {
                    DataPager.MoveToPage(currentPage - 1);
                    SelectPageAndUpdateVisualStates();
                }
            }
        }

        private void OnMoveToLastPageButtonClick(object sender, RoutedEventArgs e)
        {
            DataPager.MoveToLastPage();
            SelectPageAndUpdateVisualStates();
        }

        private void OnMoveToNextPageButtonClick(object sender, RoutedEventArgs e)
        {
            DataPager.MoveToNextPage();
            SelectPageAndUpdateVisualStates();
        }

        private void OnMoveToPreviousPageButtonClick(object sender, RoutedEventArgs e)
        {
            DataPager.MoveToPreviousPage();
            SelectPageAndUpdateVisualStates();
        }

        private void OnMoveToFirstPageButtonClick(object sender, RoutedEventArgs e)
        {
            DataPager.MoveToFirstPage();
            SelectPageAndUpdateVisualStates();
        }

        private void SelectNumericButton()
        {
            foreach (NumericButton button in numericButtons.Items)
            {
                if (!button.IsEllipsisButton)
                {
                    if (button.PageIndex == DataPager.PageIndex)
                    {
                        button.IsChecked = true;
                        return;
                    }
                }
            }

            CreateNumericButtons(DataPager.PageIndex);
        }

        private void UpdateVisualStates()
        {
            if (DataPager.CanMoveToPreviousPage)
            {
                this.moveToFirstPageButton.IsEnabled = true;
                this.moveToPreviousPageButton.IsEnabled = true;
            }
            else
            {
                this.moveToFirstPageButton.IsEnabled = false;
                this.moveToPreviousPageButton.IsEnabled = false;
            }

            if (DataPager.CanMoveToNextPage)
            {
                this.moveToNextPageButton.IsEnabled = true;
                this.moveToLastPageButton.IsEnabled = true;
            }
            else
            {
                this.moveToNextPageButton.IsEnabled = false;
                this.moveToLastPageButton.IsEnabled = false;
            }
        }

        private void CreateNumericButtons(int pageIndex)
        {
            // numericbuttoncount 5
            // rank       [1 2 3 4 .5.]
            // page index [0 1 2 3 .4.] [.0. 4 5 6 .7.] [.4. 7 8 9 .10.]
            //            [0 1 2 3] [4 5 6] [7 8 9]

            // [0 1 2 .3.] [.0. 3 4 .5.] [.3. 5 6 .7.] [.5. 7 8 .9] [.7. 9]

            if (NumericButtonCount <= 2)
                throw new ArgumentException("NumericButtonCount requires a number greater than 2");

            numericButtons.Items.Clear();

            bool isFirstPageGroup = pageIndex <= NumericButtonCount - 2;

            var group = isFirstPageGroup ? 1 : (int)Math.Ceiling((double)pageIndex / (NumericButtonCount - 2));
            var lastPageIndexOfCurrentGroup = (NumericButtonCount - 2) * group;
            var startPageIndexOfCurrentGroup = isFirstPageGroup ? 0 : (NumericButtonCount - 2) * (group - 1) + 1;

            if (!isFirstPageGroup)
            {
                var startPageIndexOfPreviousGroup = group == 2 ? 0 : (NumericButtonCount - 2) * (group - 2) + 1;
                CreateEllipsisButton(startPageIndexOfPreviousGroup);
            }

            if (lastPageIndexOfCurrentGroup < DataPager.PageCount - 1)
            {
                int numericButtonCount = isFirstPageGroup ? NumericButtonCount - 1 : NumericButtonCount - 2;
                for (int i = 0; i < numericButtonCount; i++)
                    CreateNumericButton(i + startPageIndexOfCurrentGroup);

                CreateEllipsisButton(lastPageIndexOfCurrentGroup + 1);
            }
            else
            {
                for (int i = startPageIndexOfCurrentGroup; i < DataPager.PageCount; i++)
                    CreateNumericButton(i);
            }
        }

        private void CreateNumericButton(int pageIndex)
        {
            var button = new NumericButton
            {
                Content = $"{pageIndex + 1}",
                GroupName = groupName,
                IsEllipsisButton = false,
                PageIndex = pageIndex,
                IsChecked = pageIndex == DataPager.PageIndex,
            };
            button.Click += OnNumericButtonClick;

            numericButtons.Items.Add(button);
        }

        private void CreateEllipsisButton(int pageIndex)
        {
            var button = new NumericButton
            {
                Content = "...",
                GroupName = groupName,
                IsEllipsisButton = true,
                PageIndex = pageIndex,
                IsChecked = false,
            };
            button.Click += OnNumericButtonClick;

            numericButtons.Items.Add(button);
        }

        private void OnNumericButtonClick(object sender, RoutedEventArgs e)
        {
            var numericButton = sender as NumericButton;
            if (numericButton.IsEllipsisButton)
            {
                var pageIndex = numericButton.PageIndex + (int)Math.Ceiling((double)(NumericButtonCount - 2) / 2);
                if (pageIndex > DataPager.PageCount - 1)
                    pageIndex = DataPager.PageCount - 1;

                DataPager.MoveToPage(pageIndex);
                SelectPageAndUpdateVisualStates();
            }
            else
            {
                DataPager.MoveToPage(numericButton.PageIndex);
                SelectPageAndUpdateVisualStates();
            }
        }
    }

    public enum AutoEllipsisMode
    {
        Both,
        None
    }
}
