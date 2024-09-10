using System;
//using ElephantSDK;
using BusJam.Scripts.Runtime.LevelCreator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BusJam.Scripts.Runtime.Managers.LevelManager
{
    public class LevelManager : ILevelManager
    {
        public event Action<LevelData> OnLevelLoad;

        public bool isGamePlayable { get; set; }
        public bool isGameFinished { get; set; }
        public bool isInMenu { get; set; }
        public bool isPlayingTutorial { get; set; }

        public int _currentLevelNumber { get; set; }
        private int _fakeLevelNumber;
        private const string CURRENT_LEVEL_NUMBER_KEY = "current_level_number";
        private const string FAKE_LEVEL_NUMBER_KEY = "fake_level_number";
        private LevelData _currentLevel;

        public LevelManager()
        {
            isGamePlayable = true;
            Initialize();
        }

        private void Initialize()
        {
            _currentLevelNumber = PlayerPrefs.GetInt(CURRENT_LEVEL_NUMBER_KEY, 1);
            _fakeLevelNumber = PlayerPrefs.GetInt(FAKE_LEVEL_NUMBER_KEY, 1);
        }

        public void LoadLevelByIndex(int index)
        {
            _currentLevel = LevelSaveSystem.LoadLevel(index);
            OnLevelLoad?.Invoke(_currentLevel);
        }

        public void LoadCurrentLevel()
        {
            if (!LevelSaveSystem.IsLevelExists(_currentLevelNumber))
            {
                _currentLevelNumber = 1;
            }

            _currentLevel = LevelSaveSystem.LoadLevel(_currentLevelNumber);
            OnLevelLoad?.Invoke(_currentLevel);
        }

        public void NextLevel()
        {
            _currentLevelNumber++;
            _fakeLevelNumber++;
            if (_currentLevelNumber>= SceneManager.sceneCountInBuildSettings)
            {
                _currentLevelNumber = 1;
                if (!LevelSaveSystem.IsLevelExists(_currentLevelNumber))
                {
                    _currentLevelNumber = 1;
                }
            }

            PlayerPrefs.SetInt(CURRENT_LEVEL_NUMBER_KEY, _currentLevelNumber);
            PlayerPrefs.SetInt(FAKE_LEVEL_NUMBER_KEY, _fakeLevelNumber);

            RestartLevel();
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(_currentLevelNumber);
        }

        public void LoadLevelByIndexForSrDebugger(int levelNumber)
        {
            PlayerPrefs.SetInt(CURRENT_LEVEL_NUMBER_KEY, levelNumber);
            RestartLevel();
        }

        public int GetCurrentLevelIndex()
        {
            return _currentLevelNumber;
        }

        public LevelData GetCurrentLevelData()
        {
            return _currentLevel;
        }

        public int GetFakeLevelIndex()
        {
            return _fakeLevelNumber;
        }
    }
}