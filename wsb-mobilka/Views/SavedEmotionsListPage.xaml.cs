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
            observableCollection = new ObservableCollection<EmotionsDataWithPicture>();
            printEmotionClass = new PrintEmotionClass();

            ListBox.ItemsSource = null;
            ListBox.ItemsSource = observableCollection;
            LoadEmotionsDataToListView();

            TextBlockEmotion.DataContext = printEmotionClass;
        }

        private void OnSelectedItem(object sender, SelectionChangedEventArgs e)
        {
            double itemHapiness = observableCollection[this.ListBox.SelectedIndex].Hapiness;
            printEmotionClass.EmotionsText = itemHapiness.ToString();
        }

        private void LoadObjectsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEmotionsDataToListView();
        }

        private async void LoadEmotionsDataToListView()
        {
            EmotionsDataWithPicture emotionItem;
            MainController mainController = new MainController();
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
    }
}
