using System;

namespace BusJam.Scripts.Runtime.Managers.GameManager
{
    public enum GameState
    {
        None,
        Gameplay,
        Settings,
        LevelComplete,
        LevelFailed,
        // Todo: Add more states for your game
    }
    
    public interface IGameManager
    {
        public event Action<GameState> OnGameStateChanged; 
        public GameState CurrentGameState { get; }
        public void ChangeGameState(GameState gameState);
        public void CompleteLevel();
        public void FailLevel();
    }
}