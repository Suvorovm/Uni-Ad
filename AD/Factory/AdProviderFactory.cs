using System;
using System.Collections.Generic;
using System.Reflection;
using Ad.Descriptor;
using Ad.Provider;
using Ad.Service;

namespace Ad.Factory
{
    public class AdProviderFactory
    {
        private static readonly Dictionary<Type, Func<IProviderDescriptor, IAdAnalytics, IAdProvider>>
            _providerRegistry =
                new Dictionary<Type, Func<IProviderDescriptor, IAdAnalytics, IAdProvider>>()
                {
                    {
                        typeof(IronSourceDescriptor),
                        (descriptor, adAnalytics) => new IronSourceAdProvider(descriptor as IronSourceDescriptor, adAnalytics)
                    },
                    {
                        typeof(FakeAdDescriptor),
                        (descriptor, adAnalytics) => new FakeAdProvider(descriptor as FakeAdDescriptor)
                    }
                    // new providers can be here
                };

        public static IAdProvider CreateProvider(AdDescriptor adDescriptor, IAdAnalytics adAnalytics)
        {
            foreach (PropertyInfo property in typeof(AdDescriptor).GetProperties())
            {
                if (typeof(IProviderDescriptor).IsAssignableFrom(property.PropertyType))
                {
                    IProviderDescriptor descriptor = property.GetValue(adDescriptor) as IProviderDescriptor;
                    if (descriptor != null && descriptor.Enable)
                    {
                        if (_providerRegistry.TryGetValue(property.PropertyType, out var factory))
                        {
                            return factory(descriptor, adAnalytics);
                        }
                    }
                }
            }

            throw new NotSupportedException("No valid ad provider configuration found.");
        }

        public static void RegisterProvider<TDescriptor>(Func<IProviderDescriptor, IAdAnalytics, IAdProvider> factory)
            where TDescriptor : IProviderDescriptor
        {
            _providerRegistry[typeof(TDescriptor)] = factory;
        }
    }
}