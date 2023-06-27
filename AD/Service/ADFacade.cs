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
            if (adDescriptor.FakeADDescriptor.Enable)
            {
                return new FakeADProvider();
            }

            return null;
        }

        public UniTask Init()
        {
            return _adProvider.Init(_adDescriptor);
        }

        public UniTask<ADResult> ShowAd(ADType adType)
        {
            return _adProvider.ShowAD(adType);
        }

        public void Dispose()
        {
            _adProvider?.Dispose();
        }
    }
}