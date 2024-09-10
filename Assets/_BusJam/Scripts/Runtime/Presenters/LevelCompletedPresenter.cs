using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Models;
using BusJam.Scripts.Runtime.Presenters.Animations;
using BusJam.Scripts.Runtime.UI.UIElements;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Presenters
{
    public class LevelCompletedPresenter : BasePresenter<LevelCompletedPresenter>
    {
        [SerializeField] private UIButton continueButton;
        [SerializeField] private LevelCompletedPresenterAnimations presenterAnimations;

        private LevelCompletedModel _model;

        private new void Start()
        {
            base.Start();
            _model = new LevelCompletedModel();
            continueButton.onClick.AddListener(() =>
            {
                continueButton.interactable = false;
                presenterAnimations.PlayCoinParticleAnimation(_model.GiveReward,_model.NextLevel);

            });
            
            presenterAnimations.PlayRewardAnimation(_model.Reward);
            presenterAnimations.PlayShowAnimation();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _model.Dispose();
            continueButton.onClick.RemoveListener(_model.NextLevel);
        }
    }
}