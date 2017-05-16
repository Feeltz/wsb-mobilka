using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace wsb_mobilka.Views
{
    public sealed partial class EmotionsView : Page
    {

        private Geolocator _geolocator = null;
        private Geoposition currentPosition;
        private MainPage _rootPage = MainPage.Current;
        private CognitiveController cognitiveController;
        RandomAccessStreamReference mapIconStreamReference;

        public EmotionsView()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            cognitiveController = new CognitiveController();
            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/MapPin.png"));
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


        private async void RefreshPosition(object sender, RoutedEventArgs e)
        {
            await _checkPosition();
        }     

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            _checkPosition();
        }

        private async void TakePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            progRing.IsActive = true;
            FaceImage.Source = await cognitiveController.TakePicture();
            var emotions = await cognitiveController.DetectEmotions();

            if (emotions != null)
            {
                _setEmotionsToInterface(emotions);
                await _checkPosition();
                progRing.IsActive = false;
                MainController mainController = new MainController();
                string photoFileName = await mainController.SavePictureToFolder(cognitiveController.Photo);
                mainController.SaveCurrentEmotionsToFolder(emotions, currentPosition, photoFileName);
            }

            progRing.IsActive = false;
        }




        private void _setEmotionsToInterface(Microsoft.ProjectOxford.Common.Contract.EmotionScores emotions)
        {
            HapinessTextBlock.Text = Math.Round((double)(emotions.Happiness) * 100, 4) + " %";
            SadnessTextBlock.Text = Math.Round((double)(emotions.Sadness) * 100, 4) + " %";
            SurpriseTextBlock.Text = Math.Round((double)(emotions.Surprise) * 100, 4) + " %";
            NeutralTextBlock.Text = Math.Round((double)(emotions.Neutral) * 100, 4) + " %";
            AngerTextBlock.Text = Math.Round((double)(emotions.Anger) * 100, 4) + " %";
            ContemptTextBlock.Text = Math.Round((double)(emotions.Contempt) * 100, 4) + " %";
            DisgustTextBlock.Text = Math.Round((double)(emotions.Disgust) * 100, 4) + " %";
            FearTextBlock.Text = Math.Round((double)(emotions.Fear) * 100, 4) + " %";

            var bestEmotion = emotions.ToRankedList().FirstOrDefault().Key;
            BestEmotionTextBlock.Text = $"You are {bestEmotion}!";

            _setEmotionIcon(bestEmotion);
        }

        private void _setEmotionIcon(string bestEmotion)
        {
            Emotionaaaa tmpEmot = (Emotionaaaa)Enum.Parse(typeof(Emotionaaaa), bestEmotion, true);
            string pictureUrl = @"ms-appx:/Assets/Emoji/" + cognitiveController.EmojiDictionary[tmpEmot].ToString();
            EmotionIcon.Source = new BitmapImage(new Uri(pictureUrl, UriKind.Absolute));
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

                    _updateLocationData(currentPosition);
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
            _setPinOnMap(geoPos);
        }

        private void _updateLocationData(Geoposition position)
        {
            if (position == null)
            {
                NaviX.Text = "No data";
                NaviY.Text = "No data";
            }
            else
            {
                NaviX.Text = position.Coordinate.Point.Position.Latitude.ToString();
                NaviY.Text = position.Coordinate.Point.Position.Longitude.ToString();
            }
        }

        private void _setPinOnMap(Geopoint pos)
        {
            MapIcon mapIcon1 = new MapIcon();
            Geopoint geoPos = pos;
            mapIcon1.Location = geoPos;
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = "My Position";
            mapIcon1.Image = mapIconStreamReference;
            mapIcon1.ZIndex = 0;

            if(myMap.MapElements.Count>0)
            {
                myMap.MapElements.Remove(myMap.MapElements.FirstOrDefault());
            }
            myMap.MapElements.Add(mapIcon1);
        }
    }
}
