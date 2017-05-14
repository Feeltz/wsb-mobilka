using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace wsb_mobilka
{
    
    public class CognitiveController
    {
        private readonly EmotionServiceClient emotionServiceClient = new EmotionServiceClient("dc67427c656c41899ccd6ce3891cbb02"); // AZURE
        private StorageFile photo;
        public Dictionary<Emotionaaaa, string> EmojiDictionary { get; set; }

        public async Task<EmotionScores> DetectEmotions()
        {
            Emotion[] emotionResult;
            var storageFile = photo;
            
            var randomAccessStream = await storageFile.OpenReadAsync();
            emotionResult = await emotionServiceClient.RecognizeAsync(randomAccessStream.AsStream());
            EmotionScores score = emotionResult[0].Scores;
            
            return score;
        }
        public CognitiveController()
        {
            EmojiDictionary = new Dictionary<Emotionaaaa, string>
            {
                { Emotionaaaa.Happiness, "emojiHappy.png" },
                { Emotionaaaa.Sadness, "emojiSad.png" },
                { Emotionaaaa.Suprise, "emojiSuprised.png" },
                { Emotionaaaa.Neutral, "emojiNautral.png" },
                { Emotionaaaa.Contempt, "emojiContempt.png" },
                { Emotionaaaa.Anger, "emojiAnger.png" },
                { Emotionaaaa.Disgust, "emojiDisgust.png" },
                { Emotionaaaa.Fear, "emojiFear.png" }

            };

        }

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

            return bitmapSource;
        }
    }

   
    
}
