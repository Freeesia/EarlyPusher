using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using SFLibs.Core.IO;
using Windows.Storage;

namespace SFLibs.UWP.IO
{
    public static class XmlSerializeExtension
    {
        public static async Task Save<T>(this StorageFile file, T obj)
        {
            var s = XmlSerializerUtil.GetSerializer(typeof(T));
            var setting = new XmlWriterSettings
            {
                NewLineOnAttributes = true,
                Indent = true,
            };
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var st = await file.OpenStreamForWriteAsync())
            using (var xmlWriter = XmlWriter.Create(st, setting))
            {
                s.Serialize(xmlWriter, obj);
            }
        }

        public static async Task<T> Load<T>(this StorageFile file)
        {
            var s = XmlSerializerUtil.GetSerializer(typeof(T));

            using (var st = await file.OpenStreamForReadAsync())
            {
                try
                {
                    return (T)s.Deserialize(st);
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
        }
    }
}
