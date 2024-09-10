using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace LocalizationSystem
{
    public class LocalizationManager : ILocalizationManager
    {
        private Dictionary<string, string> _localizedText;
        private string _currentLanguage;

        public void Init(string language)
        {
            _currentLanguage = language;
            LoadLocalization();
        }
    
        private void LoadLocalization()
        {
            _localizedText = new Dictionary<string, string>();
            var filePath = GetLocalizationPath();

            if (File.Exists(filePath))
            {
                string jsonOutput = File.ReadAllText(filePath);
                _localizedText = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonOutput);
            }
            else
            {
                Debug.LogError($"Cannot find localization file at {filePath}");
            }
        }

        public string Get(string key)
        {
            return _localizedText.TryGetValue(key, out var value) ? value : "Key not found";
        }
        
        public string Get(string key, params object[] args)
        {
            return _localizedText.TryGetValue(key, out var value) ? string.Format(value, args) : "Key not found";
        }

        private string GetLocalizationPath()
        {
            return Path.Combine(Application.dataPath, $"LocalizationSystem\\Languages\\{_currentLanguage}.json");
        }
    }
}

