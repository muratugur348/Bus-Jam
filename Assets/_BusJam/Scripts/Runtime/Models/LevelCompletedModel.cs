using BusJam.Scripts.Runtime.Managers.CurrencyManager;
using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Managers.SoundManager;

namespace BusJam.Scripts.Runtime.Models
{
    public class LevelCompletedModel : BaseModel<LevelCompletedModel>
    {
        private readonly ILevelManager _levelManager;
        private readonly ISoundManager _soundManager;
        private readonly ICurrencyManager _currencyManager;

        public int Reward { get; private set; }

        public LevelCompletedModel()
        {
            _levelManager = Locator.Instance.Resolve<ILevelManager>();
            _soundManager = Locator.Instance.Resolve<ISoundManager>();
            _currencyManager = Locator.Instance.Resolve<ICurrencyManager>();
            _soundManager.PlaySound("level_completed");
            Reward = BusManager.Instance.busColors.Length * 3;
        }

        public void GiveReward()
        {
            _currencyManager.AddCurrency("softCurrency", Reward);
        }

        public void NextLevel() => _levelManager.NextLevel();
    }
}