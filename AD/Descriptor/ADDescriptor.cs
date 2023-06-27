using System.Xml.Serialization;

namespace AD.Descriptor
{
    [XmlRoot("adConfig")]
    public class ADDescriptor
    {
        [XmlElement("fakeAd")]
        public FakeADDescriptor FakeADDescriptor { get; set; }
    }
}