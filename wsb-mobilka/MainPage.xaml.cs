using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            user = new UserProfile();
            _loadUserDataFromLocalStore();
        }

        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.UserProfilePage),user);
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
    }
}
