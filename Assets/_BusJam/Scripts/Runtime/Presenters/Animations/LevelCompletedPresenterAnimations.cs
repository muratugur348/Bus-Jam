using System;
using System.Collections;
using AssetKits.ParticleImage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using BusJam.Scripts.Runtime.Managers.HapticManager;
using BusJam.Scripts.Runtime.Managers.SoundManager;
using BusJam.Scripts.Runtime.UI.UIElements.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusJam.Scripts.Runtime.Presenters.Animations
{
    public class LevelCompletedPresenterAnimations : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private RectTransform headerHolder;
        [SerializeField] private RectTransform continueButton;
        [SerializeField] private RectTransform emojiPanel;
        [SerializeField] private UIButtonAnimations continueButtonAnimations;
        [SerializeField] private TextMeshProUGUI currencyAmountLabel;
        [SerializeField] private ParticleImage coinParticleImage;

        private Sequence _sequence;
        private int _currentRewardAmount;
        private bool _isCompletedOnce;

        private ISoundManager _soundManager;
        private IHapticManager _hapticManager;


        private void Start()
        {
            _soundManager = Locator.Instance.Resolve<ISoundManager>();
            _hapticManager = Locator.Instance.Resolve<IHapticManager>();
        }

        public void PlayShowAnimation()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(background.DOFade(0.7f, 0.5f));
            _sequence.Append(headerHolder.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));
            _sequence.Append(emojiPanel.DOScale(1, 0.25f).SetEase(Ease.OutBack));
            _sequence.Append(continueButton.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(0.2f));
            _sequence.OnComplete(() =>
            {
                continueButtonAnimations.PlayLoopAnimation();
                _sequence.Kill();
            });
        }

        public void PlayCoinParticleAnimation(Action onFirstParticleFinished, Action onComplete)
        {
            _soundManager.PlaySound("coin");
            _hapticManager.TriggerHaptic(HapticType.LightImpact);
            continueButtonAnimations.StopLoopAnimation();
            coinParticleImage.Play();
            coinParticleImage.onFirstParticleFinish.AddListener(() => { _soundManager.PlaySound("coin2");
                StartCoroutine(PlayCoinHaptic());
            });
            coinParticleImage.onLastParticleFinish.AddListener(OnLastParticleFinished);
            coinParticleImage.onFirstParticleFinish.AddListener(() => onFirstParticleFinished?.Invoke());

            async void OnLastParticleFinished()
            {
                await UniTask.Delay(1000);
                if (_isCompletedOnce == false)
                {
                    _isCompletedOnce = true;
                    onComplete?.Invoke();
                }
            }
        }
        private IEnumerator PlayCoinHaptic()
        {
            int count = 5;
            while (count > 0)
            {
                _hapticManager.TriggerHaptic(HapticType.LightImpact);
                count--;
                yield return new WaitForSeconds(0.05f);
            }
        }


        public void PlayRewardAnimation(int amount)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => _currentRewardAmount, x => _currentRewardAmount = x, amount, 0.5f).OnUpdate(() =>
            {
                currencyAmountLabel.text = _currentRewardAmount.ToString();
            })).SetEase(Ease.Linear);
            sequence.Append(currencyAmountLabel.transform.DOScale(1.3f, 0.25f).SetEase(Ease.OutBack));
            sequence.Append(currencyAmountLabel.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack));
            sequence.OnComplete(() => { sequence.Kill(); });
        }

        private void OnDestroy()
        {
            coinParticleImage.onLastParticleFinish.RemoveAllListeners();
            coinParticleImage.onFirstParticleFinish.RemoveAllListeners();
            _sequence?.Kill();
        }
    }
}