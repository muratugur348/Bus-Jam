using BusJam.Scripts.Runtime.Models;
using BusJam.Scripts.Runtime.Presenters.Animations;
using BusJam.Scripts.Runtime.UI.UIElements;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Presenters
{
    public class LevelFailedPresenter :  BasePresenter<LevelFailedPresenter>
    {
        [SerializeField] private UIButton retryButton;
        [SerializeField] private LevelFailedPresenterAnimations presenterAnimations;

        private LevelFailedModel _model;
        
        private new void Start()
        {
            base.Start();
            _model = new LevelFailedModel();
            retryButton.onClick.AddListener(_model.RestartLevel);
            presenterAnimations.PlayShowAnimation();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _model.Dispose();
            retryButton.onClick.RemoveListener(_model.RestartLevel);
        }
    }
}