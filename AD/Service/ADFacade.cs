using System;
using AD.Descriptor;
using AD.Model;
using AD.Provider;
using CGK.Descriptor.Service;
using Cysharp.Threading.Tasks;

namespace AD.Service
{
    public class ADFacade : IDisposable
    {
        private readonly IADProvider _adProvider;
        private readonly ADDescriptor _adDescriptor;
        
        public ADFacade(DescriptorService descriptorService)
        {
            _adDescriptor = descriptorService.GetDescriptor<ADDescriptor>();
            _adProvider = CreateProvider(_adDescriptor);
        }

        private IADProvider CreateProvider(ADDescriptor adDescriptor)
        {
#if UNITY_EDITOR
            return new FakeADProvider();
#endif
            if (adDescriptor.IronSourceDescriptor.Enable)
            {
                return new IronSourceAdProvider();
            }

            return new FakeADProvider();
        }

        public UniTask Init()
        {
            return _adProvider.Init(_adDescriptor);
        }

        public async UniTask<ADResult> ShowAd(ADType adType, string placement)
        {
            ADResult adResult = await _adProvider.ShowAD(adType, placement);
            return adResult;
        }

        public void Dispose()
        {
            _adProvider?.Dispose();
        }
    }
}