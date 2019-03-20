﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MvvmLib.Navigation;

namespace AdaptiveSample.Views
{
    /// <summary>
    /// Logique d'interaction pour AsControlView.xaml
    /// </summary>
    public partial class Scenario3 : UserControl
    {
        IRegionManager regionManager;

        public Scenario3(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            InitializeComponent();
        }

        private async void OnGoBack(object sender, RoutedEventArgs e)
        {
            await regionManager.GetContentRegion("Main").GoBackAsync();
        }
    }
}
