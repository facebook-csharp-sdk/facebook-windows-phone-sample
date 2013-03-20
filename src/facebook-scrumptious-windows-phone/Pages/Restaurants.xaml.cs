using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using System.Collections.ObjectModel;
using Facebook.Scrumptious.WindowsPhone.ViewModel;

namespace Facebook.Scrumptious.WindowsPhone.Pages
{
    public partial class Restaurants : PhoneApplicationPage
    {


        public Restaurants()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (this.restaurantsListBox.SelectedIndex >= 0)
            {
                FacebookData.SelectedRestaurant = (Location)this.restaurantsListBox.SelectedItem;
                FacebookData.IsRestaurantSelected = true;
            }

            base.OnNavigatedFrom(e);
        }
    }
}