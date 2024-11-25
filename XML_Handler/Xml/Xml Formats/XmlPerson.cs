namespace XML_Handler
{
    internal class XmlPerson
    {
        public XmlPerson(XmlFullName? fullName, string? address, XmlDistrict? district, string? date)
        {
            this.fullName   = fullName;
            this.address    = address;
            this.district   = district;
            this.date       = date;
        }

        public XmlFullName? fullName {  get; set; }
        public string? address { get; set; }
        public XmlDistrict? _district = new();
        public XmlDistrict? district {
            get
            {
                return _district;
            }
            set
            {
                int position = value!.name.IndexOf(" ");
                _district!.name = value.name.Substring(position + 1);
            }
        }

        public string? department { get; set; }

        private DateOnly? _date { get; set; }
        public string? date
        {
            get 
            {
                if (_date.HasValue)
                    return _date.Value.ToString("yyyy.MM.dd");
                else
                    return null;
            }

            set 
            {
                if (DateOnly.TryParse(value, out DateOnly tempDate)) 
                    _date = tempDate;
            }
        }

        public bool AllFieldsHasValue()
        {
            if ( (fullName != null) && (address != null) && (district != null) && (_date != null))
                return true;
            else
                return false;
        }
    }
}
