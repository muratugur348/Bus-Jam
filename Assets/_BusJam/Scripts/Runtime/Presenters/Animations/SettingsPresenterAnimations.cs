using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BusJam.Scripts.Runtime.Presenters.Animations
{
    public class SettingsPresenterAnimations : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private RectTransform settingsPanel;
        [SerializeField] private RectTransform closeButton;
        [SerializeField] private RectTransform headerHolder;

        private Sequence _sequence;
        
        public void PlayShowAnimation()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(background.DOFade(0.5f, 0.2f));
            _sequence.Join(settingsPanel.DOScale(1, 0.3f).SetEase(Ease.OutBack)).SetDelay(0.1f);
            _sequence.Append(headerHolder.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));
            _sequence.Append(closeButton.DOScale(1, 0.3f).SetEase(Ease.OutBack));
            _sequence.OnComplete(() => _sequence.Kill());
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}