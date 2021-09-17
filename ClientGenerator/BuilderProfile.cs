using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace ClientGenerator
{
    class BuilderProfile
    {
        private readonly string _profilePath;

        public string StrInject
        {
            get
            {
                return ReadValueSafe("StrInject", "-- Safe Injected Value --");
            }
            set
            {
                WriteValue("StrInject", value.ToString());
            }
        }

        public BuilderProfile(string profileName)
        {
            if (string.IsNullOrEmpty(profileName)) throw new ArgumentException("Invalid Profile Path");
            _profilePath = Path.Combine(Application.StartupPath, "Profiles\\" + profileName + ".xml");
        }

        private string ReadValue(string pstrValueToRead)
        {
            try
            {
                XPathDocument doc = new XPathDocument(_profilePath);
                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr = nav.Compile(@"/settings/" + pstrValueToRead);
                XPathNodeIterator iterator = nav.Select(expr);
                while (iterator.MoveNext())
                {
                    return iterator.Current.Value;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ReadValueSafe(string pstrValueToRead, string defaultValue = "")
        {
            string value = ReadValue(pstrValueToRead);
            return (!string.IsNullOrEmpty(value)) ? value : defaultValue;
        }

        private void WriteValue(string pstrValueToRead, string pstrValueToWrite)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                if (File.Exists(_profilePath))
                {
                    using (var reader = new XmlTextReader(_profilePath))
                    {
                        doc.Load(reader);
                    }
                }
                else
                {
                    var dir = Path.GetDirectoryName(_profilePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    doc.AppendChild(doc.CreateElement("settings"));
                }

                XmlElement root = doc.DocumentElement;
                XmlNode oldNode = root.SelectSingleNode(@"/settings/" + pstrValueToRead);
                if (oldNode == null) // create if not exist
                {
                    oldNode = doc.SelectSingleNode("settings");
                    oldNode.AppendChild(doc.CreateElement(pstrValueToRead)).InnerText = pstrValueToWrite;
                    doc.Save(_profilePath);
                    return;
                }
                oldNode.InnerText = pstrValueToWrite;
                doc.Save(_profilePath);
            }
            catch
            {
            }
        }
    }
}
