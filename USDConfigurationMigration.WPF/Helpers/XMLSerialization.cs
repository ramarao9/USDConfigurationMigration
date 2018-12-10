
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace USDConfigurationMigration.WPF.Helpers
{
    public class XMLSerialization
    {


        public string Serialize<T>(T val)
        {
            string xml = null;
            XmlSerializer xsSerializer = new XmlSerializer(typeof(T));

            try
            {
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSerializer.Serialize(writer, val);
                        xml = sww.ToString();
                    }
                }
            }
            catch
            {

            }
            return xml;
        }



        public T DeSerialize<T>(string path)
        {
            T val = default(T);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    val = (T)xmlSerializer.Deserialize(reader);
                    reader.Close();
                }
            }
            catch
            {

            }

            return val;
        }

    }
}
