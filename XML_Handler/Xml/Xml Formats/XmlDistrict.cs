using System.Xml.Serialization;

namespace XML_Handler
{
    [Serializable, XmlRoot(ElementName = "Район", Namespace = "http://rosmintrud.ru/ut/2022-07-14")]
    public class XmlDistrict
    {
        [XmlElement(ElementName = "Название", Namespace = "http://rosmintrud.ru/ut/2022-07-14")]
        public string name { get; set; }
    }
}