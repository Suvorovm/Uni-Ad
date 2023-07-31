using AD.Model;

namespace AD.Service
{
    public interface IADAnalytics
    {
        void SendAdEvent(string placement, ADResult result, ADType adType);
    }
}