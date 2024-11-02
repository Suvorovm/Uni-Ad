using System.Xml.Serialization;

namespace Ad.Model
{
    public enum AdResult
    {
        [XmlEnum("notInitialized")]
        NotInitialized,

        [XmlEnum("failLoad")]
        FailLoad,

        [XmlEnum("failShow")]
        FailShow,

        [XmlEnum("networkError")]
        NetworkError,

        [XmlEnum("adClosed")]
        AdClosed,

        [XmlEnum("adNotReady")]
        AdNotReady,

        [XmlEnum("successfully")]
        Successfully
    }
}