using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScrumptiousWindowsPhone.ViewModel;

namespace ScrumptiousWindowsPhone.Pages
{
    public partial class MealSelector : PhoneApplicationPage
    {
        public MealSelector()
        {
            InitializeComponent();
        }

        private void MealSelectionListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MealSelectionListBox.SelectedItem != null)
            {
                FacebookData.SelectedMeal = (Meal)this.MealSelectionListBox.SelectedItem;
            }
        }
    }
}