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
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace wsb_mobilka.Views
{
    public class PrintEmotionClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private string _emotionsText;
        public string EmotionsText
        {
            get
            {
                return this._emotionsText;
            }

            set
            {
                if (value != this._emotionsText)
                {
                    this._emotionsText = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SavedEmotionsListPage : Page
    {
        public ObservableCollection<EmotionsDataWithPicture> observableCollection;
        public PrintEmotionClass printEmotionClass;

        public SavedEmotionsListPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            observableCollection = new ObservableCollection<EmotionsDataWithPicture>();
            printEmotionClass = new PrintEmotionClass();

            TextBlockEmotion.DataContext = printEmotionClass;

            ListBox.ItemsSource = null;
            ListBox.ItemsSource = observableCollection;
            LoadEmotionsDataToListView();

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

        private void OnSelectedItem(object sender, SelectionChangedEventArgs e)
        {
            var item = observableCollection[this.ListBox.SelectedIndex];
            string tmpString = "Your Emotions are : \n" +

                "Happiness: " + Math.Round((double)(item.Hapiness) * 100, 4) + " %" + "\n" +

                "Sadness: " + Math.Round((double)(item.Sadness) * 100, 4) + " %" + "\n" +

                "Surprise: " + Math.Round((double)(item.Suprise) * 100, 4) + " %" + "\n" +

                "Neutral: " + Math.Round((double)(item.Neutral) * 100, 4) + " %" + "\n" +

                "Anger: " + Math.Round((double)(item.Anger) * 100, 4) + " %" + "\n" +

                "Contempt: " + Math.Round((double)(item.Contempt) * 100, 4) + " %" + "\n" +

                "Disgust: " + Math.Round((double)(item.Disgust) * 100, 4) + " %" + "\n" +

                "Fear: " + Math.Round((double)(item.Fear) * 100, 4) + " %" + "\n";
            printEmotionClass.EmotionsText = tmpString;
        }

        private void LoadObjectsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEmotionsDataToListView();
        }

        private async void LoadEmotionsDataToListView()
        {
            EmotionsDataWithPicture emotionItem;
            MainController mainController = new MainController();
            try
            {
                StorageFolder filesFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("EmotionDataFolder");
                var allFiles = await filesFolder.GetFilesAsync();
                foreach (var file in allFiles)
                {
                    var emotionFile = await mainController.LoadEmotionFromFolder(file.Name);
                    StorageFile photoFile = await mainController.LoadPictureFromFolder(emotionFile.PhotoFileName);
                    SoftwareBitmapSource photoSource = await mainController.GetBitmapSourceFromFile(photoFile);
                    emotionItem = new EmotionsDataWithPicture(emotionFile);
                    emotionItem.PhotoSource = photoSource;
                    observableCollection.Add(emotionItem);
                }
            }
            catch
            {

            }
            //emotionItem = new EmotionsDataWithPicture();
            //emotionItem.Sadness = 5;
            //emotionItem.BestEmotionName = "Sadness";
            //observableCollection.Add(emotionItem);
        }
    }
}
