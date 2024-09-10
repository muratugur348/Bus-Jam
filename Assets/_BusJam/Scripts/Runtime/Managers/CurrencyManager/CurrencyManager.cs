using System;
using System.Collections.Generic;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Managers.CurrencyManager
{
    public class CurrencyData
    {
        public string Key;
        public int Amount;
    }
    
    public class CurrencyManager : ICurrencyManager
    {
        public event Action<CurrencyData> OnCurrencyChanged;
        
        private readonly List<CurrencyData> _currencyDataList = new();

        public CurrencyManager()
        {
            CurrencyData softCurrency = new CurrencyData()
            {
                Key = "softCurrency",
                Amount = PlayerPrefs.GetInt("softCurrency", 0)
            };
            _currencyDataList.Add(softCurrency);
        }

        public void AddCurrency(string key, int amount)
        {
            var currencyData = _currencyDataList.Find(x => x.Key == key);
            if (currencyData == null)
            {
                currencyData = new CurrencyData();
                currencyData.Key = key;
                currencyData.Amount = amount;
                _currencyDataList.Add(currencyData);
            }
            else
            {
                currencyData.Amount += amount;
            }
            
            Save();
            OnCurrencyChanged?.Invoke(currencyData);
        }

        public void RemoveCurrency(string key, int amount)
        {
            var currencyData = _currencyDataList.Find(x => x.Key == key);
            if (currencyData == null)
            {
                currencyData = new CurrencyData();
                currencyData.Key = key;
                currencyData.Amount = amount;
                _currencyDataList.Add(currencyData);
            }
            else
            {
                currencyData.Amount -= amount;
            }
            
            Save();
            OnCurrencyChanged?.Invoke(currencyData);
        }

        public CurrencyData GetCurrency(string key)
        {
            var currencyData = _currencyDataList.Find(x => x.Key == key);
            return currencyData;
        }

        public int GetCurrencyAmount(string key)
        {
            var currencyData = _currencyDataList.Find(x => x.Key == key);
            return currencyData.Amount;
        }


        private void Save()
        {
            foreach (var currencyData in _currencyDataList)
            {
                PlayerPrefs.SetInt(currencyData.Key, currencyData.Amount);
            }
        }
    }
}