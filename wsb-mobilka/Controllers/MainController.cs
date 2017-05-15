using Microsoft.ProjectOxford.Common.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace wsb_mobilka
{
    public class MainController
    {

        private StorageFile photo;
        public SoftwareBitmapSource photoSoftwareBitmapSource;
        public async Task<SoftwareBitmapSource> TakePicture()
        {

            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);
            captureUI.PhotoSettings.AllowCropping = true;

            // nasza apka czeka, na zrobienie zdjęcia prze aplikację CameraCaptureUI
            photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // Gdy klikniemy cancel na aplikacji do robienia zdjęć
                return null;
            }

            // ciąg bitów ze zdjęcia
            IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
            // dekodujemy na bitmapę
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            // zmieniamy rodzaj bitmapy na software
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            // konwertujemy bitmape na taką, która nam odpowiada do tego zadania
            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            // ustawaimy obiekt typu źródło bitmap i wciskamy tam nasze wcześniej skonwertowane zdjęcie
            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();

            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);
            photoSoftwareBitmapSource = bitmapSource;

            return bitmapSource;
        }


        public async Task<SoftwareBitmapSource> LoadPhotoFromStorage()
        {
            try
            {
                StorageFolder destinationFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("ProfilePhotoFolder");
                photo = await destinationFolder.GetFileAsync("ProfilePhoto.jpg");
            }
            catch
            {

            }
            if (photo == null)
            {
                return null;
            }
            
            return await GetBitmapSourceFromFile(photo);
        }

        public async void SavePictureInLocalStorage()
        {
            if(photo == null)
            {
                return;
            }

            StorageFolder destinationFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfilePhotoFolder", CreationCollisionOption.OpenIfExists);
            await photo.CopyAsync(destinationFolder, "ProfilePhoto.jpg", NameCollisionOption.ReplaceExisting);
        }

        public async Task<string> SavePictureToFolder(StorageFile photoFile, string folderName="EmotionPictures")
        {
            if (photoFile == null)
            {
                return null;
            }

            StorageFolder destinationFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            var xxx = await destinationFolder.GetFilesAsync();
            string photoFileName = "EmotionPicture" + (xxx.Count + 1) + ".jpg";
            var photoname = await photoFile.CopyAsync(destinationFolder, photoFileName, NameCollisionOption.GenerateUniqueName);

            return photoFileName;
        }

        public async Task<StorageFile> LoadPictureFromFolder(string fileName, string folderName = "EmotionPictures")
        {
            StorageFolder destinationFolder = null;
            StorageFile photoFile = null;
            try
            {
                destinationFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(folderName);
                photoFile = await destinationFolder.GetFileAsync(fileName);
            }
            catch
            {
                return null;
            }
                        
            return photoFile;
        }

        public async void SaveCurrentEmotionsToFolder(EmotionScores emotions, Geoposition currentPosition, string photoFileName, string folderName = "EmotionDataFolder")
        {
            var currentEmotionData = new EmotionsData()
            {
                Anger = emotions.Anger,
                Sadness = emotions.Sadness,
                Neutral = emotions.Neutral,
                Fear = emotions.Fear,
                Disgust = emotions.Disgust,
                Contempt = emotions.Contempt,
                Hapiness = emotions.Happiness,
                Suprise = emotions.Surprise,
            };

            var bestEmotionKey = emotions.ToRankedList().FirstOrDefault().Key;
            var bestEmotionValue = emotions.ToRankedList().FirstOrDefault().Value;
            currentEmotionData.BestEmotionName = bestEmotionKey;
            currentEmotionData.BestEmotionScore = bestEmotionValue;
            //currentEmotionData.position = currentPosition;
            currentEmotionData.Longitude = currentPosition.Coordinate.Longitude;
            currentEmotionData.Latitude = currentPosition.Coordinate.Latitude;
            currentEmotionData.PhotoFileName = photoFileName;

            currentEmotionData.DateTaken = DateTime.Now;
            string fileName = DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".xml";
            await SerializationController.SaveObjectToXml<EmotionsData>(currentEmotionData, fileName, folderName);
        }

        public async Task<EmotionsData> LoadEmotionFromFolder(string emotionFileName, string folderName = "EmotionDataFolder")
        {
            var currentEmotionData = new EmotionsData();
            currentEmotionData = await SerializationController.ReadObjectFromXmlFileAsync<EmotionsData>(emotionFileName, folderName);

            return currentEmotionData;
        }

        public async Task<SoftwareBitmapSource> GetBitmapSourceFromFile(StorageFile photoFile)
        {
            IRandomAccessStream stream = await photoFile.OpenAsync(FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

            return bitmapSource;
        }

    }
}
