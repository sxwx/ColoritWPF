﻿using System.Windows;

namespace ColoritWPF.Views
{
    /// <summary>
    /// Interaction logic for AddNewDinsityItem.xaml
    /// </summary>
    public partial class AddNewDensityItem : Window
    {
        public AddNewDensityItem()
        {
            InitializeComponent();
        }
        
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
