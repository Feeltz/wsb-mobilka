using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace wsb_mobilka
{
    public static class SerializationController
    {
        public static async Task<T> ReadObjectFromXmlFileAsync<T>(string filename, string folderName)
        {
            T objectFromXml = default(T);
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(folderName);
            StorageFile file = await folder.GetFileAsync(filename);
            Stream stream = await file.OpenStreamForReadAsync();
            objectFromXml = (T)serializer.Deserialize(stream);
            stream.Dispose();
            return objectFromXml;
        }

        public static async Task SaveObjectToXml<T>(T objectToSave, string filename, string folderName)
        {
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            Stream stream = await file.OpenStreamForWriteAsync();

            using (stream)
            {
                serializer.Serialize(stream, objectToSave);
            }
        }
    }
}
