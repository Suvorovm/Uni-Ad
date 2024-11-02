using System;
using Ad.Model;
using Cysharp.Threading.Tasks;

namespace Ad.Provider
{
    public interface IAdProvider : IDisposable
    {
        UniTask Init();
        UniTask<AdResult> ShowAd(AdType adType, string placement = "");
        void DestroyBanner();
    }
}