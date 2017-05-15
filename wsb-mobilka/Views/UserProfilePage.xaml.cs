using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.UI.Core;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace wsb_mobilka.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserProfilePage : Page
    {
        public UserProfile user;
        MainController controler;
        private bool isPictureTaken=false;

        public UserProfilePage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            user = new UserProfile();
            var _enumval = Enum.GetValues(typeof(Gender)).Cast<Gender>();
            GenderComboBox.ItemsSource = _enumval.ToList();

            controler = new MainController();
            _loadPhotoFromStorage();
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
            user = e.Parameter as UserProfile;
            _setUserProfileControlsFromUser();

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

        
        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _saveUserDataToLocalStore();
            if(isPictureTaken)
                controler.SavePictureInLocalStorage();
        }
        private  void TakePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            _takePhoto();
            isPictureTaken = true;
        }


        private void _saveUserDataToLocalStore()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["UserProfileFirstName"] = this.FirstNameTextBlock.Text;
            localSettings.Values["UserProfileLastName"] = this.LastNameTextBlock.Text;
            localSettings.Values["UserProfileAge"] = this.AgeTextBlock.Text;
            localSettings.Values["UserProfileGender"] = this.GenderComboBox.SelectedValue.ToString();
        }
        private async void _takePhoto()
        {
            FaceImage.Source = await controler.TakePicture();
        }
        private void _setUserProfileControlsFromUser()
        {
            try
            {
                this.FirstNameTextBlock.Text = user.FirstName;
                this.LastNameTextBlock.Text = user.LastName;
                this.AgeTextBlock.Text = user.Age.ToString();

                int i = 0;
                foreach (var item in GenderComboBox.Items)
                {
                    if (item.ToString() == user.Gender.ToString())
                    {
                        this.GenderComboBox.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            { }
        }
        private async void _loadPhotoFromStorage()
        {
            FaceImage.Source = await controler.LoadPhotoFromStorage();
        }






















    }
}
