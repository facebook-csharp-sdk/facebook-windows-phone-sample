using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using Facebook;

namespace facebook_windows_phone_sample.Pages
{
    public partial class FacebookInfoPage : PhoneApplicationPage
    {
        private string _accessToken;
        private string _userId;

        public FacebookInfoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _accessToken = NavigationContext.QueryString["access_token"];
            _userId = NavigationContext.QueryString["id"];
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFacebookData();
        }

        private void LoadFacebookData()
        {
            GetUserProfilePicture();

            GraphApiSample();

            FqlSample();
        }

        private void GetUserProfilePicture()
        {
            // available picture types: square (50x50), small (50xvariable height), large (about 200x variable height) (all size in pixels)
            // for more info visit http://developers.facebook.com/docs/reference/api
            string profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", _userId, "square", _accessToken);

            picProfile.Source = new BitmapImage(new Uri(profilePictureUrl));
        }

        private void GraphApiSample()
        {
            var fb = new FacebookClient(_accessToken);

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
                    ProfileName.Text = "Hi " + (string)result["name"];
                    FirstName.Text = "First Name: " + (string)result["first_name"];
                    FirstName.Text = "Last Name: " + (string)result["last_name"];
                });
            };

            fb.GetAsync("me");
        }

        private void FqlSample()
        {
            var fb = new FacebookClient(_accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                var data = (IList<object>)result["data"];

                var count = data.Count;

                // since this is an async callback, make sure to be on the right thread
                // when working with the UI.
                Dispatcher.BeginInvoke(() =>
                {
                    TotalFriends.Text = string.Format("You have {0} friend(s).", count);
                });
            };

            // query to get all the friends
            var query = string.Format("SELECT uid,pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1={0})", "me()");

            // Note: For windows phone 7, make sure to add [assembly: InternalsVisibleTo("Facebook")] if you are using anonymous objects as parameter.
            fb.GetAsync("fql", new { q = query });
        }

        private string _lastMessageId;
        private void PostToWall_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtMessage.Text))
            {
                MessageBox.Show("Enter message.");
                return;
            }

            var fb = new FacebookClient(_accessToken);

            fb.PostCompleted += (o, args) =>
            {
                if (args.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)args.GetResultData();
                _lastMessageId = (string)result["id"];

                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Message Posted successfully");

                    txtMessage.Text = string.Empty;
                    btnDeleteLastMessage.IsEnabled = true;
                });
            };

            var parameters = new Dictionary<string, object>();
            parameters["message"] = txtMessage.Text;

            fb.PostAsync("me/feed", parameters);
        }

        private void DeleteLastMessage_Click(object sender, RoutedEventArgs e)
        {
            btnDeleteLastMessage.IsEnabled = false;

            var fb = new FacebookClient(_accessToken);

            fb.DeleteCompleted += (o, args) =>
            {
                if (args.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                    return;
                }

                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Message deleted successfully");
                    btnDeleteLastMessage.IsEnabled = false;
                });
            };

            fb.DeleteAsync(_lastMessageId);
        }
    }
}