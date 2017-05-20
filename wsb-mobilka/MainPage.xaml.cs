using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace wsb_mobilka
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public UserProfile user;
        public static MainPage Current;
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Current = this;
            user = new UserProfile();
            _loadUserDataFromLocalStore();
        }

        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _loadUserDataFromLocalStore()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            try
            {
                user.FirstName = localSettings.Values["UserProfileFirstName"].ToString();
                user.LastName = localSettings.Values["UserProfileLastName"].ToString();
                user.Age = Convert.ToInt32(localSettings.Values["UserProfileAge"].ToString());

                Gender tmpGender;
                Enum.TryParse(localSettings.Values["UserProfileGender"].ToString(), out tmpGender);
                user.Gender = tmpGender;
            }
            catch(Exception exc)
            {
                
            }
            
            //localSettings.Values["UserProfileGender"] = this.GenderComboBox.SelectedValue.ToString();
        }

        public void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }

            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void EmotionseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.EmotionsView), user);
        }

        private void SavedEmotionsListPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.SavedEmotionsListPage), user);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EnterProfile_Click(object sender, RoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(Views.UserProfilePage), user);
        }
    }
}
