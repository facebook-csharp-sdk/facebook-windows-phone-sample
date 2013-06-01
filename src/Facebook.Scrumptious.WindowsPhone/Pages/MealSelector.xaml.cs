using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Facebook.Scrumptious.WindowsPhone.ViewModel;

namespace Facebook.Scrumptious.WindowsPhone.Pages
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