using BusJam.Scripts.Runtime.Models;
using BusJam.Scripts.Runtime.Presenters.Animations;
using BusJam.Scripts.Runtime.UI.UIElements;
using UniRx;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Presenters
{
    public class SettingsPresenter : BasePresenter<SettingsPresenter>
    {
        [SerializeField] private SettingsPresenterAnimations presenterAnimations;
        [SerializeField] private UIButton privacyPolicyButton;
        [SerializeField] private UIToggleButton soundToggle;
        [SerializeField] private UIToggleButton hapticToggle;
        private SettingsModel _model;
        
        private new void Start()
        {
            base.Start();
            _model = new SettingsModel();
            //privacyPolicyButton.onClick.AddListener(_model.ShowPrivacyPolicy);
            presenterAnimations.PlayShowAnimation();
            _model.IsSoundOn.Subscribe(OnSoundStateChanged);
            _model.IsHapticOn.Subscribe(OnHapticStateChanged);
            soundToggle.onValueChanged.AddListener(_model.ToggleSound);
            hapticToggle.onValueChanged.AddListener(_model.ToggleHaptic);
        }
        
        private void OnSoundStateChanged(bool isOn) =>soundToggle.isOn = isOn;
        private void OnHapticStateChanged(bool isOn) =>  hapticToggle.isOn = isOn;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _model.Dispose();
            //privacyPolicyButton.onClick.RemoveListener(_model.ShowPrivacyPolicy);
            soundToggle.onValueChanged.RemoveListener(_model.ToggleSound);
            hapticToggle.onValueChanged.RemoveListener(_model.ToggleHaptic);
        }
    }
}