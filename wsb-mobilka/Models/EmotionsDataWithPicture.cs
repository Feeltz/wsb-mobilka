using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Media.Imaging;

namespace wsb_mobilka
{
    

    public class EmotionsDataWithPicture: EmotionsData
    {
        public SoftwareBitmapSource PhotoSource { get; set; }

        public EmotionsDataWithPicture() { }
        public EmotionsDataWithPicture(EmotionsData EmotionDataFile)
        {
            this.Anger = EmotionDataFile.Anger;
            this.Sadness = EmotionDataFile.Sadness;
            this.Neutral = EmotionDataFile.Neutral;
            this.Suprise = EmotionDataFile.Suprise;
            this.Fear = EmotionDataFile.Fear;
            this.Contempt = EmotionDataFile.Contempt;
            this.Disgust = EmotionDataFile.Disgust;
            this.BestEmotionName = EmotionDataFile.BestEmotionName;
            this.BestEmotionScore = EmotionDataFile.BestEmotionScore;
            this.Latitude = EmotionDataFile.Latitude;
            this.Longitude = EmotionDataFile.Longitude;
            this.DateTaken = EmotionDataFile.DateTaken;
            this.PhotoFileName = EmotionDataFile.PhotoFileName;
        }
    }
}
