using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BusJam.Scripts.Runtime.Presenters.Animations
{
    public class GameplayPresenterAnimations : MonoBehaviour
    {
        [SerializeField] private RectTransform currencyHolder;
        [SerializeField] private RectTransform levelInfoHolder;
        [SerializeField] private RectTransform settingsButton;
        [SerializeField] private Image background;

        public void PlayShowAnimation(Action onAnimationsComplete)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Join(background.DOFade(0f, 0.7f).SetEase(Ease.OutSine));
            sequence.Append(currencyHolder.DOAnchorPosX(350, 0.5f).SetEase(Ease.OutSine));
            sequence.Join(levelInfoHolder.DOAnchorPosX(450, 0.5f).SetEase(Ease.OutSine).SetDelay(0.1f));
            sequence.Join(settingsButton.DOAnchorPosX(-425f, 0.2f).SetEase(Ease.OutSine).SetDelay(0.1f));
            sequence.OnComplete(() => onAnimationsComplete?.Invoke());
        }
    }
}