using System.Xml.Serialization;
using System.Xml;

namespace XML_Handler.Xml
{
    internal class Deserializer
    {
        private readonly string _xmlPath = "";

        // пространства имён, использующиеся в xml файлах комитета
        public static string mt = "http://rosmintrud.ru/mse/mt/2022-10-28";
        public static string ut = "http://rosmintrud.ru/ut/2022-07-14";
        public Deserializer(string xmlPath)
        {
            _xmlPath = xmlPath;
        }

        public string? GetOneValue(string nameSpace, string name)
        {
            using XmlReader reader = XmlReader.Create(_xmlPath);
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.LocalName == name && reader.NamespaceURI == nameSpace)
                    return reader.ReadElementContentAsString();
            }

            throw new Exception($"Не удалось получить значение поля \"{name}\"");
        }

        public object? Deserialize<T>(string nameSpace, string name)
        {
            try
            {
                XmlSerializer xmlSerializer = new(typeof(T));

                using XmlReader reader = XmlReader.Create(_xmlPath);
                reader.ReadToDescendant(name, nameSpace);
                var temp = reader.ReadSubtree();
                return xmlSerializer.Deserialize(temp);
            }
            catch
            {
                throw new Exception($"Не удалось получить значение поля \"{name}\"");
            }
        }
    }
}
