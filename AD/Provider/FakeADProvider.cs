using System;
using System.Threading;
using AD.Descriptor;
using AD.Model;
using AD.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AD.Provider
{
    public class FakeADProvider : IADProvider
    {
        private float _timeOutLoadingRewardVideo;
        private float _timeShowingRewardVideo;
        private float _timeShowingInterstitial;
        private ADResult _resultRewardVideoShowed;
        private ADResult _resultShowingInterstitial;

        private FakeDialogController _fakeDialogController;
        private CancellationTokenSource _cancellationTokenSource;

        public UniTask Init(ADDescriptor adDescriptor)
        {
            _timeOutLoadingRewardVideo = adDescriptor.FakeADDescriptor.TimeOutLoadingAd;
            _resultRewardVideoShowed = adDescriptor.FakeADDescriptor.ResultShowingReward;
            _timeShowingRewardVideo = adDescriptor.FakeADDescriptor.TimeShowingReward;
            _resultShowingInterstitial = adDescriptor.FakeADDescriptor.ResultShowingReward;
            _timeShowingInterstitial = adDescriptor.FakeADDescriptor.TimeShowingInterstitial;
            CreateFakeDialog(adDescriptor);

            return UniTask.CompletedTask;
        }

        private void CreateFakeDialog(ADDescriptor adDescriptor)
        {
            
            FakeDialogController fakeDialogPrefab = Resources.Load<FakeDialogController>(adDescriptor.FakeADDescriptor.PathToDialog);
            _fakeDialogController = Object.Instantiate(fakeDialogPrefab);
            _fakeDialogController.Hide();
            _fakeDialogController.OnADResult
                .Subscribe(_ => { _cancellationTokenSource?.Cancel(); });
        }

        public async UniTask<ADResult> ShowAD(ADType adType, string result)
        {
            float timeShowing = SelectTimeToShow(adType);
            ADResult showResult = SelectShowResult(adType);
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
                return ADResult.AdClosed;
            }

            return showResult;
        }

        private ADResult SelectShowResult(ADType adType)
        {
            return adType == ADType.Reward ? _resultRewardVideoShowed : _resultShowingInterstitial;
        }

        private float SelectTimeToShow(ADType adType)
        {
            return adType == ADType.Reward ? _timeShowingRewardVideo : _timeShowingInterstitial;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }
    }
}