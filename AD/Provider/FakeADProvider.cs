using System;
using System.Threading;
using Ad.Descriptor;
using Ad.Model;
using Ad.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ad.Provider
{
    public class FakeAdProvider : IAdProvider
    {
        private readonly FakeAdDescriptor _fakeAdDescriptor;
        
        private float _timeOutLoadingRewardVideo;
        private float _timeShowingRewardVideo;
        private float _timeShowingInterstitial;
        private AdResult _resultRewardVideoShowed;
        private AdResult _resultShowingInterstitial;

        private FakeDialogController _fakeDialogController;
        private CancellationTokenSource _cancellationTokenSource;

        private GameObject _banner;

        public FakeAdProvider(FakeAdDescriptor adDescriptor)
        {
            _fakeAdDescriptor = adDescriptor;
        }
        public UniTask Init()
        {
            _timeOutLoadingRewardVideo = _fakeAdDescriptor.TimeOutLoadingAd;
            _resultRewardVideoShowed = _fakeAdDescriptor.ResultShowingReward;
            _timeShowingRewardVideo = _fakeAdDescriptor.TimeShowingReward;
            _resultShowingInterstitial = _fakeAdDescriptor.ResultShowingReward;
            _timeShowingInterstitial = _fakeAdDescriptor.TimeShowingInterstitial;
            CreateFakeDialog();
            CreateBanner();
            
            return UniTask.CompletedTask;
        }

        private void CreateBanner()
        {
            _banner = GameObject.Instantiate(Resources.Load<GameObject>(_fakeAdDescriptor.PathToBanner));
            _banner.SetActive(false);
        }

        private void CreateFakeDialog()
        {
            FakeDialogController fakeDialogPrefab = Resources.Load<FakeDialogController>(_fakeAdDescriptor.PathToDialog);
            _fakeDialogController = Object.Instantiate(fakeDialogPrefab);
            _fakeDialogController.Hide();
            _fakeDialogController.OnADResult
                .Subscribe(_ =>
                {
                    _cancellationTokenSource?.Cancel();
                });
        }

        public async UniTask<AdResult> ShowAd(AdType adType, string result)
        {
            if (adType == AdType.BannerBottom || adType == AdType.BannerTop)
            {
                ShowBanner();
                return AdResult.Successfully;
            }
            float timeShowing = SelectTimeToShow(adType);
            AdResult showResult = SelectShowResult(adType);
            _fakeDialogController.Show(timeShowing, adType);
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_timeShowingRewardVideo), true,
                    cancellationToken: _cancellationTokenSource.Token);
                _fakeDialogController.Hide();
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log(ex.Message);
                _fakeDialogController.Hide();
                return AdResult.AdClosed;
            }

            return showResult;
        }

        private void ShowBanner()
        {
            _banner.SetActive(true);
        }

        public void DestroyBanner()
        {
            Debug.LogWarning("Destroy banner");
            _banner.SetActive(false);
        }

        private AdResult SelectShowResult(AdType adType)
        {
            return adType == AdType.Reward ? _resultRewardVideoShowed : _resultShowingInterstitial;
        }

        private float SelectTimeToShow(AdType adType)
        {
            return adType == AdType.Reward ? _timeShowingRewardVideo : _timeShowingInterstitial;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }
    }
}