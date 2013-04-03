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
using Facebook;
using Facebook.Scrumptious.WindowsPhone.ViewModel;
using Facebook.Client;
using System.Threading.Tasks;

namespace Facebook.Scrumptious.WindowsPhone.Pages
{
    public partial class FacebookLoginPage : PhoneApplicationPage
    {
        private readonly string AppId = Constants.FacebookAppId;

        /// <summary>
        /// Extended permissions is a comma separated list of permissions to ask the user.
        /// </summary>
        /// <remarks>
        /// For extensive list of available extended permissions refer to 
        /// https://developers.facebook.com/docs/reference/api/permissions/
        /// </remarks>
        private const string ExtendedPermissions = "user_about_me,read_stream";

        private readonly FacebookClient _fb = new FacebookClient();

        public FacebookLoginPage()
        {
            InitializeComponent();
            this.Loaded += FacebookLoginPage_Loaded;
        }

        async void FacebookLoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.isAuthenticated)
            {
                App.isAuthenticated = true;
                await Authenticate();
            }
        }

        private FacebookSession session;
        private async Task Authenticate()
        {
            string message = String.Empty;
            try
            {
                session = await App.FacebookSessionClient.LoginAsync("user_about_me,read_stream");
                App.AccessToken = session.AccessToken;
                App.FacebookId = session.FacebookId;

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Pages/LandingPage.xaml", UriKind.Relative)));
            }
            catch (InvalidOperationException e)
            {
                message = "Login failed! Exception details: " + e.Message;
                MessageBox.Show(message);
            }
        }
    }
}