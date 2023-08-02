using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Kodprov_Softhouse
{
    [XmlType(TypeName = "person")]
    public class Person
    {
        public string firstname;
        public string lastname;
        public Address address;
        public Phone phone;
        [XmlElement("family")]
        public List<Family> family = new List<Family>();
    }
}
