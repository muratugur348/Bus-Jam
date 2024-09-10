using BusJam.Scripts.Runtime.Managers.GameManager;
using BusJam.Scripts.Runtime.Managers.HapticManager;
using BusJam.Scripts.Runtime.Managers.SoundManager;
using UniRx;

namespace BusJam.Scripts.Runtime.Models
{
    public class SettingsModel : BaseModel<SettingsModel>
    {
        public ReactiveProperty<bool> IsSoundOn { get; }
        public ReactiveProperty<bool> IsHapticOn { get; }

        private readonly IGameManager _gameManager;
        private readonly ISoundManager _soundManager;
        private readonly IHapticManager _hapticManager;

        public SettingsModel()
        {
            _gameManager = Locator.Instance.Resolve<IGameManager>();
            _soundManager = Locator.Instance.Resolve<ISoundManager>();
            _hapticManager = Locator.Instance.Resolve<IHapticManager>();

            IsSoundOn = new ReactiveProperty<bool>(_soundManager.IsSoundOn);
            IsHapticOn = new ReactiveProperty<bool>(_hapticManager.IsHapticOn);
        }

        public void ToggleSound(bool isOn) => _soundManager.ToggleSounds(isOn);
        public void ToggleHaptic(bool isOn) => _hapticManager.ToggleHaptic(isOn);

        public override void Dispose()
        {
            _gameManager.ChangeGameState(GameState.Gameplay);
            IsSoundOn.Dispose();
            IsHapticOn.Dispose();
        }
    }
}