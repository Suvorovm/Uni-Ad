using System;
using Ad.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Ad.UI
{
    public class FakeDialogController : MonoBehaviour
    {
        [SerializeField]
        private Button _closeButton;
        [SerializeField]
        private Text _timeToClose;

        private readonly Subject<AdResult> _subjectAdResult = new Subject<AdResult>();
        private CompositeDisposable _compositeDisposable;
        public IObservable<AdResult> OnADResult => _subjectAdResult.AsObservable();

        private void Awake()
        {
            _closeButton.OnClickAsObservable()
                .Subscribe(_ => { _subjectAdResult.OnNext(AdResult.AdClosed); })
                .AddTo(gameObject);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _compositeDisposable?.Dispose();
            _compositeDisposable = null;
        }

        public void Show(float showingTime, AdType adType)
        {
            gameObject.SetActive(true);
            _timeToClose.text = $"{adType} {showingTime}";
            _compositeDisposable?.Dispose();
            _compositeDisposable = new CompositeDisposable();
            float timer = showingTime;
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    timer -= 1;
                    _timeToClose.text = $"{adType} {timer}";
                })
                .AddTo(_compositeDisposable);
        }
    }
}