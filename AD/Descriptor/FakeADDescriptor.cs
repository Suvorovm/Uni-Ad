using System.Xml.Serialization;
using AD.Model;

namespace AD.Descriptor
{
    [XmlType("fakeAd")]
    public class FakeADDescriptor
    {
        [XmlAttribute("timoOutLoadingAd")] public float TimeOutLoadingAd { get; set; }
        [XmlAttribute("timeShowingReward")] public float TimeShowingReward { get; set; }
        [XmlAttribute("resultShowingReward")] public ADResult ResultShowingReward { get; set; }

        [XmlAttribute("resultShowingInterstitial")]
        public ADResult InterstitialInterstitial { get; set; }

        [XmlAttribute("timeShowingInterstitial")]
        public float TimeShowingInterstitial { get; set; }

        [XmlAttribute("pathToDialog")] public string PathToDialog { get; set; }
        [XmlAttribute("enable")] public bool Enable { get; set; }
    }
}