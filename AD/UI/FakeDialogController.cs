using System;
using AD.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AD.UI
{
    public class FakeDialogController : MonoBehaviour
    {
        [SerializeField]
        private Button _closeButton;
        [SerializeField]
        private Text _timeToClose;

        private readonly Subject<ADResult> _subjectAdResult = new Subject<ADResult>();
        private CompositeDisposable _compositeDisposable;
        public IObservable<ADResult> OnADResult => _subjectAdResult.AsObservable();

        private void Awake()
        {
            _closeButton.OnClickAsObservable()
                .Subscribe(_ => { _subjectAdResult.OnNext(ADResult.AdClosed); })
                .AddTo(gameObject);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _compositeDisposable?.Dispose();
            _compositeDisposable = null;
        }

        public void Show(float showingTime, ADType adType)
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