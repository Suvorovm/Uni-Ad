using System.Xml.Serialization;

namespace AD.Descriptor
{
    [XmlType("ironSource")]
    public class IronSourceDescriptor
    {
        [XmlAttribute("appToken")] public string Token { get; set; }
        [XmlAttribute("enable")] public bool Enable { get; set; }
        
        [XmlAttribute("preInitInterstitial")] public bool PreInitInterstitial { get; set; }
    }
}