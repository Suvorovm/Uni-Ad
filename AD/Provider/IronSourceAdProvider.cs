using System.Threading;
using System.Threading.Tasks;
using AD.Descriptor;
using AD.Model;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace AD.Provider
{
    //ERROR codes https://developers.is.com/ironsource-mobile/ios/supersonic-sdk-error-codes/
    public class IronSourceAdProvider : IADProvider
    {
        private static bool _initialized; // can't call IronSource.Agent.init(_ironSourceDescriptor.Token) twice
        private UniTaskCompletionSource _taskCompletionSource;
        private UniTaskCompletionSource<ADResult> _adResult;
        private IronSourceDescriptor _ironSourceDescriptor;
        private bool _interstitialRequested;


        public async UniTask Init(ADDescriptor adDescriptor)
        {
            _ironSourceDescriptor = adDescriptor.IronSourceDescriptor;
            IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitializationCompletedEvent;
            SubscribeOnEvents();

            if (_initialized)
            {
                await UniTask.CompletedTask;
                return;
            }

            IronSource.Agent.validateIntegration();
            IronSource.Agent.init(_ironSourceDescriptor.Token);
            _taskCompletionSource = new UniTaskCompletionSource();

            int indexCompletedTask =
                await UniTask.WhenAny(
                    UniTask.Delay(TimeSpan.FromSeconds(adDescriptor.IronSourceDescriptor.InitTimeOut)),
                    _taskCompletionSource.Task);
            if (indexCompletedTask == 0 && adDescriptor.IronSourceDescriptor.ThrowErrorInInit)
            {
                throw new TimeoutException("Pluggin init timeout");
            }

            await Task.CompletedTask;
        }

        private void SubscribeOnEvents()
        {
            IronSourceRewardedVideoEvents.onAdClosedEvent += OnRewardedVideoAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewardedVideoEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnFailLoadEvent;


            IronSourceInterstitialEvents.onAdClosedEvent += OnInterstitialClosed;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += OnInterstitialLoadFailed;
            IronSourceInterstitialEvents.onAdShowFailedEvent += OnInterstitialShowedFailed;
            IronSourceInterstitialEvents.onAdReadyEvent += OnInterstitialReady;
        }

        private void OnInterstitialReady(IronSourceAdInfo obj)
        {
            _interstitialRequested = false;
        }

        private void OnInterstitialShowedFailed(IronSourceError error, IronSourceAdInfo info)
        {
            if (error.getCode() == 520)
            {
                _adResult?.TrySetResult(ADResult.NetworkError);
            }
            else if (error.getCode() == 509)
            {
                _adResult?.TrySetResult(ADResult.FailLoad);
            }
            else
            {
                _adResult?.TrySetResult(ADResult.FailShow);
            }

            TryLoadInterstitial();
            _adResult = null;
        }

        private void OnInterstitialLoadFailed(IronSourceError obj)
        {
            TryLoadInterstitial();
        }

        private void OnInterstitialClosed(IronSourceAdInfo obj)
        {
            TryLoadInterstitial();
            _adResult?.TrySetResult(ADResult.AdClosed);
            _adResult = null;
        }

        private void TryLoadInterstitial()
        {
            if (IronSource.Agent.isInterstitialReady() || _interstitialRequested)
            {
                return;
            }

            IronSource.Agent.loadInterstitial();
            _interstitialRequested = true;
        }

        private void OnFailLoadEvent(IronSourceError error, IronSourceAdInfo info)
        {
            _adResult?.TrySetResult(ADResult.FailShow);
            _adResult = null;
        }

        private void OnRewardedVideoEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            Debug.Log("Showed reward");
            _adResult?.TrySetResult(ADResult.Successfully);
            _adResult = null;
        }

        public async UniTask<ADResult> ShowAD(ADType adType, string placement)
        {
            Debug.Log("ShowAD");

            if (!_initialized)
            {
                return ADResult.NotInitialized;
            }

            _adResult?.TrySetCanceled(new CancellationToken(true));
            _adResult = new UniTaskCompletionSource<ADResult>();
            if (adType == ADType.Reward)
            {
                ShowRewardVideo(placement);
            }
            else
            {
                ShowInterstitial(placement);
            }

            ADResult adResultTask = await _adResult.Task;
            _adResult = null;
            return adResultTask;
        }

        private void ShowInterstitial(string placement)
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.showInterstitial(placement);
            }
            else
            {
                TryLoadInterstitial();
                _adResult?.TrySetResult(ADResult.AdNotReady);
            }
        }

        private void ShowRewardVideo(string placement)
        {
            IronSource.Agent.showRewardedVideo(placement);
        }

        private void OnSdkInitializationCompletedEvent()
        {
            Debug.Log("On AD SdkInitializationCompleted");
            _taskCompletionSource.TrySetResult();
            _initialized = true;
            if (_ironSourceDescriptor.PreInitInterstitial)
            {
                TryLoadInterstitial();
            }
        }

        public void Dispose()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent -= OnSdkInitializationCompletedEvent;


            IronSourceRewardedVideoEvents.onAdClosedEvent -= OnRewardedVideoAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= OnRewardedVideoEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent -= OnFailLoadEvent;


            IronSourceInterstitialEvents.onAdClosedEvent -= OnInterstitialClosed;
            IronSourceInterstitialEvents.onAdLoadFailedEvent -= OnInterstitialLoadFailed;
            IronSourceInterstitialEvents.onAdShowFailedEvent -= OnInterstitialShowedFailed;
            IronSourceInterstitialEvents.onAdReadyEvent -= OnInterstitialReady;
        }

        private void OnRewardedVideoAdClosedEvent(IronSourceAdInfo info)
        {
            _adResult?.TrySetResult(ADResult.AdClosed);
            _adResult = null;
        }
    }
}