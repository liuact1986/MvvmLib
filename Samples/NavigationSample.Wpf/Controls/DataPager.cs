using MvvmLib.Navigation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace NavigationSample.Wpf.Controls
{
    public class DataPager : Control
    {
        private const string MoveToFirstPageButtonPartName = "MoveToFirstPageButton";
        private const string MoveToPreviousPageButtonPartName = "MoveToPreviousPageButton";
        private const string MoveToNextPageButtonPartName = "MoveToNextPageButton";
        private const string MoveToLastPageButtonPartName = "MoveToLastPageButton";
        private const string CurrentPageTextBoxPartName = "CurrentPageTextBox";
        private const string NumericButtonsPartName = "NumericButtons";
        private Button moveToFirstPageButton;
        private Button moveToPreviousPageButton;
        private Button moveToNextPageButton;
        private Button moveToLastPageButton;
        private TextBox currentPageTextBox;
        private ItemsControl numericButtons;
        private string groupName;

        public IPagedSource PagedSource
        {
            get { return (IPagedSource)GetValue(PagedSourceProperty); }
            set { SetValue(PagedSourceProperty, value); }
        }

        public static readonly DependencyProperty PagedSourceProperty =
            DependencyProperty.Register("PagedSource", typeof(IPagedSource), typeof(DataPager), new PropertyMetadata(null));

        public int NumericButtonCount
        {
            get { return (int)GetValue(NumericButtonCountProperty); }
            set { SetValue(NumericButtonCountProperty, value); }
        }

        public static readonly DependencyProperty NumericButtonCountProperty =
            DependencyProperty.Register("NumericButtonCount", typeof(int), typeof(DataPager), new PropertyMetadata(0));

        public AutoEllipsisMode AutoEllipsisMode
        {
            get { return (AutoEllipsisMode)GetValue(AutoEllipsisModeProperty); }
            set { SetValue(AutoEllipsisModeProperty, value); }
        }

        public static readonly DependencyProperty AutoEllipsisModeProperty =
            DependencyProperty.Register("AutoEllipsisMode", typeof(AutoEllipsisMode), typeof(DataPager), new PropertyMetadata(AutoEllipsisMode.None));


        static DataPager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataPager), new FrameworkPropertyMetadata(typeof(DataPager)));
        }

        public DataPager()
        {
            this.groupName = Guid.NewGuid().ToString();
            this.Loaded += OnLoaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.moveToFirstPageButton != null)
                this.moveToFirstPageButton.Click -= OnMoveToFirstPageButtonClick;
            if (this.moveToPreviousPageButton != null)
                this.moveToPreviousPageButton.Click -= OnMoveToPreviousPageButtonClick;
            if (this.moveToNextPageButton != null)
                this.moveToNextPageButton.Click -= OnMoveToNextPageButtonClick;
            if (this.moveToLastPageButton != null)
                this.moveToLastPageButton.Click -= OnMoveToLastPageButtonClick;
            if (this.currentPageTextBox != null)
                this.currentPageTextBox.KeyUp -= OnCurrentPageTextBoxKeyUp;

            this.moveToFirstPageButton = this.GetTemplateChild(MoveToFirstPageButtonPartName) as Button;
            this.moveToPreviousPageButton = GetTemplateChild(MoveToPreviousPageButtonPartName) as Button;
            this.moveToNextPageButton = GetTemplateChild(MoveToNextPageButtonPartName) as Button;
            this.moveToLastPageButton = GetTemplateChild(MoveToLastPageButtonPartName) as Button;
            this.currentPageTextBox = GetTemplateChild(CurrentPageTextBoxPartName) as TextBox;
            this.numericButtons = GetTemplateChild(NumericButtonsPartName) as ItemsControl;
        }

        private bool IsInDesignMode(DependencyObject element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }

        private void HandleEvents()
        {
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
                if (this.PagedSource == null)
                    throw new ArgumentNullException(nameof(PagedSource));

                HandleEvents();

                Refresh();

                this.PagedSource.Refreshed += OnRefreshed;
                this.PagedSource.PageChanged += OnPageChanged;
            }
        }

        private void OnPageChanged(object sender, PageChangeEventArgs e)
        {
            SelectPageAndUpdateButtonStates();
        }

        private void OnRefreshed(object sender, EventArgs e)
        {
            Refresh();
        }

        private void SelectPageAndUpdateButtonStates()
        {
            if (AutoEllipsisMode == AutoEllipsisMode.Both)
                SelectNumericButton();
            UpdateButtonStates();
        }

        public void Refresh()
        {
            if (AutoEllipsisMode == AutoEllipsisMode.Both)
                CreateNumericButtons(0);

            UpdateButtonStates();
        }

        private void OnCurrentPageTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (int.TryParse(textBox.Text, out int currentPage))
                {
                    PagedSource.MoveToPage(currentPage - 1);
                    SelectPageAndUpdateButtonStates();
                }
            }
        }

        private void OnMoveToLastPageButtonClick(object sender, RoutedEventArgs e)
        {
            PagedSource.MoveToLastPage();
            SelectPageAndUpdateButtonStates();
        }

        private void OnMoveToNextPageButtonClick(object sender, RoutedEventArgs e)
        {
            PagedSource.MoveToNextPage();
            SelectPageAndUpdateButtonStates();
        }

        private void OnMoveToPreviousPageButtonClick(object sender, RoutedEventArgs e)
        {
            PagedSource.MoveToPreviousPage();
            SelectPageAndUpdateButtonStates();
        }

        private void OnMoveToFirstPageButtonClick(object sender, RoutedEventArgs e)
        {
            PagedSource.MoveToFirstPage();
            SelectPageAndUpdateButtonStates();
        }

        private void SelectNumericButton()
        {
            foreach (NumericButton button in numericButtons.Items)
            {
                if (!button.IsEllipsisButton)
                {
                    if (button.PageIndex == PagedSource.PageIndex)
                    {
                        button.IsChecked = true;
                        return;
                    }
                }
            }

            CreateNumericButtons(PagedSource.PageIndex);
        }

        private void UpdateButtonStates()
        {
            if (PagedSource.CanMoveToPreviousPage)
            {
                this.moveToFirstPageButton.IsEnabled = true;
                this.moveToPreviousPageButton.IsEnabled = true;
            }
            else
            {
                this.moveToFirstPageButton.IsEnabled = false;
                this.moveToPreviousPageButton.IsEnabled = false;
            }

            if (PagedSource.CanMoveToNextPage)
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

            if (lastPageIndexOfCurrentGroup < PagedSource.PageCount - 1)
            {
                int numericButtonCount = isFirstPageGroup ? NumericButtonCount - 1 : NumericButtonCount - 2;
                for (int i = 0; i < numericButtonCount; i++)
                    CreateNumericButton(i + startPageIndexOfCurrentGroup);

                CreateEllipsisButton(lastPageIndexOfCurrentGroup + 1);
            }
            else
            {
                for (int i = startPageIndexOfCurrentGroup; i < PagedSource.PageCount; i++)
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
                IsChecked = pageIndex == PagedSource.PageIndex,
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
                if (pageIndex > PagedSource.PageCount - 1)
                    pageIndex = PagedSource.PageCount - 1;

                PagedSource.MoveToPage(pageIndex);
                SelectPageAndUpdateButtonStates();
            }
            else
            {
                PagedSource.MoveToPage(numericButton.PageIndex);
                SelectPageAndUpdateButtonStates();
            }
        }
    }

    public enum AutoEllipsisMode
    {
        Both,
        None
    }
}
