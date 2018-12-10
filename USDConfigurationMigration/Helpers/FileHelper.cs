using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace USDConfigurationMigration.Helpers
{
    public static class FileHelper
    {

        public static void CreateXmlDocument(string xmlData)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            // Save the document to a file and auto-indent the output.
            XmlWriter writer = XmlWriter.Create("data.xml", settings);
            doc.Save(writer);
        }


        public static void CreateXmlDocument(string filePath, string xmlData)
        {
            //XmlDocument doc = new XmlDocument();


            //doc.LoadXml(xmlData);


            //XmlWriterSettings settings = new XmlWriterSettings();
            //settings.Indent = true;
            //// Save the document to a file and auto-indent the output.
            //XmlWriter writer = XmlWriter.Create(filePath, settings);
            //doc.Save(writer);


            File.WriteAllText(filePath, xmlData);
            



            
        }

       



    }
}
