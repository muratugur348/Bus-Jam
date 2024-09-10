using System;
using System.Threading.Tasks;
using BusJam.Scripts.Runtime.Managers.HapticManager;
using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Managers.ViewManager;
using BusJam.Scripts.Runtime.Presenters;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Managers.GameManager
{
    public class GameManager : IGameManager
    {
        public event Action<GameState> OnGameStateChanged;
        public GameState CurrentGameState { get; private set; }


       
        public void ChangeGameState(GameState gameState)
        {
            CurrentGameState = gameState;
            OnGameStateChanged?.Invoke(gameState);
        }


        public void CompleteLevel()
        {
            if (CurrentGameState == GameState.LevelComplete) return;
            PlayHaptic(3);
            Locator.Instance.Resolve<IViewManager>()
                .LoadView(new LoadViewParams<LevelCompletedPresenter>("LevelCompletedView"), false);
            ChangeGameState(GameState.LevelComplete);
        }


        public void FailLevel()
        {
            if (CurrentGameState == GameState.LevelFailed) return;
            PlayHaptic(2);
            Locator.Instance.Resolve<IViewManager>()
                .LoadView(new LoadViewParams<LevelCompletedPresenter>("LevelFailedView"));
            ChangeGameState(GameState.LevelFailed);
        }

        private async Task PlayHaptic(int amount)
        {
            IHapticManager _hapticManager = Locator.Instance.Resolve<IHapticManager>();
            while (amount > 0)
            {
                _hapticManager.TriggerHaptic(HapticType.LightImpact);
                amount--;
                await Task.Delay(50);
            }
        }
    }
}