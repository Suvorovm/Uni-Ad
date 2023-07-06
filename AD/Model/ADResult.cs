using System.Xml.Serialization;

namespace AD.Model
{
    public enum ADResult
    {
        [XmlEnum("notInitialized")] NotInitialized,
        [XmlEnum("failLoad")] FailLoad,
        [XmlEnum("failShow")] FailShow,
        [XmlEnum("networkError")] NetworkError,
        [XmlEnum("adClosed")] AdClosed,
        [XmlEnum("adNotReady")] AdNotReady,
        [XmlEnum("successfully")] Successfully
    }
}