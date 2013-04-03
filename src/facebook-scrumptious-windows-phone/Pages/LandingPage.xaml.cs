using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Facebook;
using Facebook.Scrumptious.WindowsPhone.ViewModel;
using System.Windows.Media.Imaging;
using System.Device.Location;
using Facebook.Client;
using System.Threading.Tasks;


namespace Facebook.Scrumptious.WindowsPhone.Pages
{
    public partial class LandingPage : PhoneApplicationPage
    {
        public LandingPage()
        {
            InitializeComponent();
            LoadUserInfo();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (FacebookData.SelectedFriends.Count > 0)
            {
                if (FacebookData.SelectedFriends.Count > 1)
                {
                    this.WithWhoTextBox.Text = String.Format("with {0} and {1} others", FacebookData.SelectedFriends[0].Name, FacebookData.SelectedFriends.Count - 1);
                }
                else
                {
                    this.WithWhoTextBox.Text = "with " + FacebookData.SelectedFriends[0].Name;
                }
            }
            else
            {
                this.WithWhoTextBox.Text = "Select Friends";
            }

            if (FacebookData.IsRestaurantSelected)
            {
                this.restaurantLocationTextBlock.Text = FacebookData.SelectedRestaurant.Name;
            }

            if (!String.IsNullOrEmpty(FacebookData.SelectedMeal.Name))
            {
                this.MealName.Text = FacebookData.SelectedMeal.Name;
            }
        }

        private void LoadUserInfo()
        {
            var fb = new FacebookClient(App.AccessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();

                Dispatcher.BeginInvoke(() =>
                {
                    //ProfileName.Text = "Hi " + (string)result["name"];
                    string myName = String.Format("{0} {1}", (string)result["first_name"], (string)result["last_name"]);
                    Uri myPictureUri = new Uri(string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", App.FacebookId, "square", App.AccessToken));

                    this.MyImage.Source = new BitmapImage(myPictureUri);
                    this.MyName.Text = myName;
                });
            };

            fb.GetAsync("me");
        }

        public GeoPosition<GeoCoordinate> GetCurrentLocation()
        {
            GeoCoordinateWatcher gcw = new GeoCoordinateWatcher();
            GeoPosition<GeoCoordinate> currentCoordinate = new GeoPosition<GeoCoordinate>();
            currentCoordinate.Location = new GeoCoordinate(47.627903, -122.143185);

            var CurrentLocation = gcw.Position;

            if (!CurrentLocation.Location.IsUnknown)
            {
                currentCoordinate.Location.Latitude = CurrentLocation.Location.Latitude;
                currentCoordinate.Location.Longitude = CurrentLocation.Location.Longitude;
            }

            return currentCoordinate;
        }



        async private void PostActionToFBHandler(object sender, EventArgs evtArgs)
        {
            if (FacebookData.SelectedFriends.Count < 1
                || FacebookData.SelectedMeal.Name == String.Empty
                || FacebookData.IsRestaurantSelected == false)
            {
                MessageBox.Show("Please select friends, a place to eat and something you ate before attempting to share!");
                return;
            }

            FacebookSession session;
            string message = String.Empty;
            try
            {
                session = await App.FacebookSessionClient.LoginAsync("publish_stream");
            }
            catch (InvalidOperationException e)
            {
                message = "Login failed! Exception details: " + e.Message;
                MessageBox.Show(message);
                return;
            }

            FacebookClient fb = new FacebookClient(App.AccessToken);

            fb.PostCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();

                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Posted Open Graph Action, id: " + (string)result["id"], "Result", MessageBoxButton.OK);

                    // reset the selections after the post action has successfully concluded
                    this.MealName.Text = "Select One";
                    this.restaurantLocationTextBlock.Text = "Select One";
                    this.WithWhoTextBox.Text = "Select Friends";
                });
            };

            fb.PostAsync( String.Format("/me/{0}:eat", Constants.FacebookAppGraphAction), new { meal = FacebookData.SelectedMeal.MealUri, tags = FacebookData.SelectedFriends[0].id, place = FacebookData.SelectedRestaurant.Id});
        }

        private void friendSelectorTextBlockHandler(object sender, System.Windows.Input.GestureEventArgs evtArgs)
        {
            FacebookClient fb = new FacebookClient(App.AccessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();

                var data = (IEnumerable<object>)result["data"]; ;


                Dispatcher.BeginInvoke(() =>
                {
                    // The observable collection can only be updated from within the UI thread. See 
                    // http://10rem.net/blog/2012/01/10/threading-considerations-for-binding-and-change-notification-in-silverlight-5
                    // If you try to update the bound data structure from a different thread, you are going to get a cross
                    // thread exception.
                    foreach (var item in data)
                    {
                        var friend = (IDictionary<string, object>)item;

                        FacebookData.Friends.Add(new Friend { Name = (string)friend["name"], id = (string)friend["id"], PictureUri = new Uri(string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", (string)friend["id"], "square", App.AccessToken)) });
                    }

                    NavigationService.Navigate(new Uri("/Pages/FriendSelector.xaml", UriKind.Relative));
                });

            };

            fb.GetAsync("/me/friends");
        }

        private void restaurantLocationTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs evtArgs)
        {
            FacebookClient fb = new FacebookClient(App.AccessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();

                var data = (IEnumerable<object>)result["data"]; ;


                Dispatcher.BeginInvoke(() =>
                {
                    // The observable collection can only be updated from within the UI thread. See 
                    // http://10rem.net/blog/2012/01/10/threading-considerations-for-binding-and-change-notification-in-silverlight-5
                    // If you try to update the bound data structure from a different thread, you are going to get a cross
                    // thread exception.
                    foreach (var item in data)
                    {
                        var restaurant = (IDictionary<string, object>)item;

                        var location = (IDictionary<string, object>)restaurant["location"];
                        //string name = (string)friend["name"];
                        FacebookData.Locations.Add(new Location
                        {
                            // the address is one level deeper within the object
                            Street = location.ContainsKey("street") ? (string)location["street"] : String.Empty,
                            City = location.ContainsKey("city") ? (string)location["city"] : String.Empty,
                            State = location.ContainsKey("state") ? (string)location["state"] : String.Empty,
                            Country = location.ContainsKey("country") ? (string)location["country"] : String.Empty,
                            Zip = location.ContainsKey("zip") ? (string)location["zip"] : String.Empty,
                            Latitude = location.ContainsKey("latitude") ? ((double)location["latitude"]).ToString() : String.Empty,
                            Longitude = location.ContainsKey("longitude") ? ((double)location["longitude"]).ToString() : String.Empty,

                            // these properties are at the top level in the object
                            Category = restaurant.ContainsKey("category") ? (string)restaurant["category"] : String.Empty,
                            Name = restaurant.ContainsKey("name") ? (string)restaurant["name"] : String.Empty,
                            Id = restaurant.ContainsKey("id") ? (string)restaurant["id"] : String.Empty,
                            PictureUri = new Uri(string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", (string)restaurant["id"], "square", App.AccessToken))
                        });
                    }

                    NavigationService.Navigate(new Uri("/Pages/Restaurants.xaml", UriKind.Relative));
                });

            };


            GeoPosition<GeoCoordinate> currentLocation = GetCurrentLocation();
            fb.GetAsync("/search", new { q = "restaurant", type = "place", center = currentLocation.Location.Latitude.ToString() + "," + currentLocation.Location.Longitude.ToString(), distance = "1000" });
        }

        private void MealName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MealSelector.xaml", UriKind.Relative));
        }
    }
}