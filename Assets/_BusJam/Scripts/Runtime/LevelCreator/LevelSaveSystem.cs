using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace BusJam.Scripts.Runtime.LevelCreator
{
    public static class LevelSaveSystem
    {
#if UNITY_EDITOR
        static readonly string LevelDataPath = Application.dataPath + $"/Resources/LevelData/";
        static readonly string BackupLevelDataPath = Application.dataPath + $"/Resources/BackupLevelData/";
#else
        static readonly string LevelDataPath = $"/LevelData/";
        static readonly string BackupLevelDataPath = $"/BackupLevelData/";
#endif
        public static LevelData LoadLevel(int levelIndex)
        {
#if UNITY_EDITOR

            if (!Directory.Exists(LevelDataPath))
            {
                Debug.LogWarning("LevelData directory not found!");
                return null;
            }

            var filePath = LevelDataPath + $"Level{levelIndex}.json";
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Level{levelIndex}.json not found in the {filePath}");
                return null;
            }

#endif

            var json = Resources.Load<TextAsset>($"LevelData/Level{levelIndex}").text;
            return JsonConvert.DeserializeObject<LevelData>(json);
        }

        public static void SaveLevel(LevelData levelGrid, int levelIndex)
        {
#if UNITY_EDITOR
            if (!Directory.Exists(LevelDataPath))
            {
                Directory.CreateDirectory(LevelDataPath);
            }

            var filePath = LevelDataPath + $"Level{levelIndex}.json";
            if (File.Exists(filePath))
            {
                BackupLevel(levelIndex);
            }

            string json = JsonConvert.SerializeObject(levelGrid);
            File.WriteAllText(filePath, json);

            AssetDatabase.Refresh();
#endif
        }

        static void BackupLevel(int levelIndex)
        {
#if UNITY_EDITOR

            if (!Directory.Exists(BackupLevelDataPath))
            {
                Directory.CreateDirectory(BackupLevelDataPath);
            }

            var backupLevel = Resources.Load<TextAsset>($"LevelData/Level{levelIndex}").text;
            var filePath = BackupLevelDataPath + $"Level{levelIndex}.json";
            File.WriteAllText(filePath, backupLevel);
#endif
        }

        public static bool IsLevelExists(int levelIndex)
        {
            return Resources.Load<TextAsset>($"LevelData/Level{levelIndex}") != null;
        }
    }
}