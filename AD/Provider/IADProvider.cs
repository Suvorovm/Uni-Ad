using System;
using AD.Descriptor;
using AD.Model;
using Cysharp.Threading.Tasks;

namespace AD.Provider
{
    public interface IADProvider : IDisposable
    {
        UniTask Init(ADDescriptor adDescriptor);
        UniTask<ADResult> ShowAD(ADType adType);
    }
}