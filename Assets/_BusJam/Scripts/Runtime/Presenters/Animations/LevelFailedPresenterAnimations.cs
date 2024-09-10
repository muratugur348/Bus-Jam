using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusJam.Scripts.Runtime.Presenters.Animations
{
    public class LevelFailedPresenterAnimations : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private RectTransform headerHolder;
        [SerializeField] private TextMeshProUGUI outOfMovesLabel;
        [SerializeField] private RectTransform retryButton;
        [SerializeField] private RectTransform emojiPanel;

        private Sequence _sequence;
        
        public void PlayShowAnimation()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(background.DOFade(0.7f, 0.5f));
            _sequence.Append(outOfMovesLabel.DOFade(1, 0.7f).SetEase(Ease.OutSine));
            _sequence.Append(outOfMovesLabel.transform.DOLocalMoveY(-240f, 0.5f).SetEase(Ease.OutSine));
            _sequence.Append(headerHolder.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));
            _sequence.Append(emojiPanel.DOScale(1, 0.25f).SetEase(Ease.OutBack));
            _sequence.Append(retryButton.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(0.2f));
            _sequence.OnComplete(() => _sequence.Kill());
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}