using BusJam.Scripts.Runtime.Models;
using BusJam.Scripts.Runtime.Presenters.Animations;
using BusJam.Scripts.Runtime.UI.UIElements;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace BusJam.Scripts.Runtime.Presenters.GameplayPresenter
{
    public class GameplayPresenter : BasePresenter<GameplayPresenter>
    {
        [SerializeField] private UIButton restartLevelButton;
        [SerializeField] private UIButton settingsButton;
        [SerializeField] private TextMeshProUGUI currentLevelLabel;
        [SerializeField] private GameplayPresenterAnimations presenterAnimations;
        [SerializeField] private CurrencyHolder currencyHolder;
        [SerializeField] private GameObject tutorialHand;

        private GameplayModel _model;

        private bool _isCoinSet;

        private new void Start()
        {
            base.Start();

            _model = new GameplayModel();
            _model.CurrentLevelIndex.Subscribe(UpdateCurrentLevelLabel);
            _model.Coin.Subscribe(UpdateCoinLabel);

            settingsButton.onClick.AddListener(_model.ShowSettings);
            restartLevelButton.onClick.AddListener(_model.RestartLevel);

            presenterAnimations.PlayShowAnimation(() =>
            {
                _model.Init();
            });
            if (TutorialManager.Instance != null)
                TutorialManager.Instance.handUI = tutorialHand;
        }

        private void UpdateCurrentLevelLabel(int levelIndex)
        {
            currentLevelLabel.text = $"Level {levelIndex}";
        }

        private void UpdateCoinLabel(int coin)
        {
            if (_isCoinSet)
            {
                currencyHolder.IncreaseCoinWithAnimation(coin);
            }
            else
            {
                _isCoinSet = true;
                currencyHolder.SetCoin(coin);
            }
        }



        protected override void OnDestroy()
        {
            base.OnDestroy();
            _model.Dispose();

            settingsButton.onClick.RemoveListener(_model.ShowSettings);
            restartLevelButton.onClick.RemoveListener(_model.RestartLevel);
        }
    }
}