using BusJam.Scripts.Runtime.Managers.CurrencyManager;
using BusJam.Scripts.Runtime.Managers.GameManager;
using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Managers.ViewManager;
using BusJam.Scripts.Runtime.Presenters;
using UniRx;

namespace BusJam.Scripts.Runtime.Models
{
    public class GameplayModel : BaseModel<GameplayModel>
    {
        public ReactiveProperty<int> Coin { get; } = new();
        public ReactiveProperty<int> CurrentLevelIndex { get; } = new();

        private readonly ILevelManager _levelManager;
        private readonly IGameManager _gameManager;
        private readonly IViewManager _viewManager;
        private readonly ICurrencyManager _currencyManager;
        

        public GameplayModel()
        {
            _levelManager = Locator.Instance.Resolve<ILevelManager>();
            _gameManager = Locator.Instance.Resolve<IGameManager>();
            _viewManager = Locator.Instance.Resolve<IViewManager>();
            _currencyManager = Locator.Instance.Resolve<ICurrencyManager>();

            _currencyManager.OnCurrencyChanged += OnCurrencyChanged;

            CurrentLevelIndex.Value = _levelManager.GetFakeLevelIndex();
            Coin.Value = _currencyManager.GetCurrencyAmount("softCurrency");

        }
        
        public void Init() =>  _gameManager.ChangeGameState(GameState.Gameplay);
        

        public void RestartLevel() => _levelManager.RestartLevel();

        private void OnCurrencyChanged(CurrencyData currencyData)
        {
            if (currencyData.Key.Equals("softCurrency"))
            {
                Coin.Value = currencyData.Amount;
            }
        }
        
        public void ShowSettings()
        {
            _gameManager.ChangeGameState(GameState.Settings);
            _viewManager.LoadView(new LoadViewParams<SettingsPresenter>("SettingsView"), false);
        }
    
        public override void Dispose()
        {
            CurrentLevelIndex?.Dispose();
        }
    }
}