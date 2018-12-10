using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace USDConfigurationMigration.Helpers
{
    public static class Extensions
    {
        public static string XmlSerialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {

                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    NewLineOnAttributes = true,
                };

                //var xmlserializer = new XmlSerializer(typeof(T));
                //var stringWriter = new XmlTextWriter();
                //using (var writer = XmlWriter.Create(stringWriter, settings))
                //{
                //    xmlserializer.Serialize(writer, value);
                //    return stringWriter.ToString();
                //}


                XmlSerializer serializer = new XmlSerializer(typeof(T));

                // create a MemoryStream here, we are just working
                // exclusively in memory
                System.IO.Stream stream = new System.IO.MemoryStream();

                // The XmlTextWriter takes a stream and encoding
                // as one of its constructors
                System.Xml.XmlTextWriter xtWriter = new System.Xml.XmlTextWriter(stream, Encoding.UTF8);

                serializer.Serialize(xtWriter, value);

                xtWriter.Flush();

                // go back to the beginning of the Stream to read its contents
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                // read back the contents of the stream and supply the encoding
                System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);

                string result = reader.ReadToEnd();
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static T XmlDeSerialize<T>(this string xmlString)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
                T resultingMessage = (T)serializer.Deserialize(memStream);
                return resultingMessage;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

    }
}
