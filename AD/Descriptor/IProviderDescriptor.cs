using System.Xml.Serialization;

namespace Ad.Descriptor
{
    public interface IProviderDescriptor
    {
        [XmlArray("enable")]
        bool Enable { get; }
    }
}