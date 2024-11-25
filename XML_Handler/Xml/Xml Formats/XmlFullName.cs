using Microsoft.Office.Interop.Word;
using System.Xml.Serialization;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace XML_Handler
{
    [Serializable, XmlRoot(ElementName = "ФИО", Namespace = "http://rosmintrud.ru/mse/mt/2022-10-28")]
    public class XmlFullName
    {
        [XmlElement(ElementName = "Фамилия", Namespace = "http://rosmintrud.ru/ut/2022-07-14")]
        public required string surname { set; get; }

        [XmlElement(ElementName = "Имя", Namespace = "http://rosmintrud.ru/ut/2022-07-14")]
        public required string name { set; get; }


        [XmlElement(ElementName = "Отчество", Namespace = "http://rosmintrud.ru/ut/2022-07-14")]
        public required string patronymic { set; get; }

        public void FixCaps()
        {
            name = FixSingleString(name);
            surname = FixSingleString(surname);
            patronymic = FixSingleString(patronymic);
        }

        private string FixSingleString(string str)
        {
            char[] letters = str.ToLower().ToCharArray();   // понижаем регистр всей строки
            letters[0] = char.ToUpper(letters[0]);          // повышаем регистр первой буквы
            return new string(letters);                     // кафуем
        }
    }
}