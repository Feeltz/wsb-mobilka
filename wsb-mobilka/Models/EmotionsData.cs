using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace wsb_mobilka
{
    

    public class EmotionsData: EmotionMainClass
    {

        //public Geoposition position { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateTaken { get; set; }
        public string PhotoFileName { get; set; }
   
    }
}
