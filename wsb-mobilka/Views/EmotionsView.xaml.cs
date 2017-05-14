using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace wsb_mobilka.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmotionsView : Page
    {

        private Geolocator _geolocator = null;
        private Geoposition currentPosition;
        private MainPage _rootPage = MainPage.Current;

        public EmotionsView()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        #region Navigation
        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            string myPages = "";
            foreach (PageStackEntry page in rootFrame.BackStack)
            {
                myPages += page.SourcePageType.ToString() + "\n";
            }

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }
        #endregion

        private void UpdateLocationData(Geoposition position)
        {
            if (position == null)
            {
                NaviX.Text = "No data";
                NaviY.Text = "No data";
                Accuracy.Text = "No data";
            }
            else
            {
                NaviX.Text = position.Coordinate.Point.Position.Latitude.ToString();
                NaviY.Text = position.Coordinate.Point.Position.Longitude.ToString();
                Accuracy.Text = position.Coordinate.Accuracy.ToString();
            }
        }

        private async void RefreshPosition(object sender, RoutedEventArgs e)
        {
            await _checkPosition();
        }

        private async Task _checkPosition()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    _geolocator = new Geolocator();
                    _geolocator.DesiredAccuracyInMeters = 50;
                    currentPosition = await _geolocator.GetGeopositionAsync();

                    UpdateLocationData(currentPosition);
                    _setCurrentPositionToMap(currentPosition);
                    break;

                case GeolocationAccessStatus.Denied:
                    _rootPage.NotifyUser("Access to location is denied.", NotifyType.ErrorMessage);
                    LocationDisabledMessage.Visibility = Visibility.Visible;
                    break;

                case GeolocationAccessStatus.Unspecified:
                    _rootPage.NotifyUser("Unspecificed error!", NotifyType.ErrorMessage);
                    LocationDisabledMessage.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void _setCurrentPositionToMap(Geoposition pos)
        {
            Geopoint geoPos = new Geopoint(new BasicGeoposition() { Latitude = pos.Coordinate.Latitude, Longitude = pos.Coordinate.Longitude });

            myMap.Center = geoPos;
            myMap.ZoomLevel = 14;
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            _checkPosition();
        }

        
    }
}
