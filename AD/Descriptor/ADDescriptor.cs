using System.Xml.Serialization;

namespace Ad.Descriptor
{
    [XmlRoot("adConfig")]
    public class AdDescriptor
    {
        [XmlElement("fakeAd")]
        public FakeAdDescriptor FakeADDescriptor { get; set; }
        
        [XmlElement("ironSource")]
        public IronSourceDescriptor IronSourceDescriptor { get; set; }
    }
}