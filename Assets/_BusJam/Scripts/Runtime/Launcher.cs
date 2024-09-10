using BusJam.Scripts.Runtime.Debugger;
using BusJam.Scripts.Runtime.Managers.CurrencyManager;
using BusJam.Scripts.Runtime.Managers.GameManager;
using BusJam.Scripts.Runtime.Managers.HapticManager;
using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Managers.SoundManager;
using BusJam.Scripts.Runtime.Managers.ViewManager;
using BusJam.Scripts.Runtime.Presenters;
using BusJam.Scripts.Runtime.Utilities;
using LocalizationSystem;
using UnityEngine;

namespace BusJam.Scripts.Runtime
{
    public class Launcher : MonoBehaviourSingleton<Launcher>
    {
        private Locator _locator;
        private IGameManager _gameManager;
        private ILevelManager _levelManager;
        private IViewManager _viewManager;
        private ILocalizationManager _localizationManager;
        private ICurrencyManager _currencyManager;
        private IHapticManager _hapticManager;

        // Mono Managers
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private bool quickInitialize;

        protected override void Awake()
        {
            base.Awake();
            InitializeManagers();
            if (!quickInitialize)
            {
                _viewManager.LoadView(new LoadViewParams<LoadingPresenter>("LoadingView"));

            }
            // TODO: Fix onLoad

            //Debug.Log("Launcher Awake");
        }

        private void InitializeManagers()
        {
            _locator = new Locator();

            // Register managers here

            _gameManager = new GameManager();
            _locator.Register<IGameManager>(_gameManager);

            _levelManager = new LevelManager();
            _locator.Register<ILevelManager>(_levelManager);

            _viewManager = new ViewManager();
            _locator.Register<IViewManager>(_viewManager);

            _localizationManager = new LocalizationManager();
            _locator.Register<ILocalizationManager>(_localizationManager);

            _currencyManager = new CurrencyManager();
            _locator.Register<ICurrencyManager>(_currencyManager);

            _hapticManager = new HapticManager();
            _hapticManager.Initialize();
            _locator.Register<IHapticManager>(_hapticManager);


            _locator.Register<ISoundManager>(soundManager);
            //Elephant.LevelStarted(_levelManager._currentLevelNumber);
        }

      

        /*private void OnDisable()
        {
            _locator?.Reset();
        }*/
    }
}