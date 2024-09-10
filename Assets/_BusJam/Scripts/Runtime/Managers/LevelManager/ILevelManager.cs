using System;
using BusJam.Scripts.Runtime.LevelCreator;

namespace BusJam.Scripts.Runtime.Managers.LevelManager
{
    public interface ILevelManager
    {
        public bool isGamePlayable { get; set; }
        public bool isInMenu { get; set; }
        public bool isGameFinished { get; set; }
        public bool isPlayingTutorial { get; set; }
        public int _currentLevelNumber { get; set; }

        public event Action<LevelData> OnLevelLoad;
        public void LoadLevelByIndex(int index);
        public void LoadCurrentLevel();
        public void NextLevel();
        public void RestartLevel();
        public int GetCurrentLevelIndex();
        public int GetFakeLevelIndex();
        public LevelData GetCurrentLevelData();
        public void LoadLevelByIndexForSrDebugger(int index);
    }
}