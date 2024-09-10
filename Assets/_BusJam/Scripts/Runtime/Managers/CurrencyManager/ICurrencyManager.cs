using System;

namespace BusJam.Scripts.Runtime.Managers.CurrencyManager
{
    public interface ICurrencyManager
    {
        public event Action<CurrencyData> OnCurrencyChanged; 
        public void AddCurrency(string key, int amount);
        public void RemoveCurrency(string key, int amount);
        public CurrencyData GetCurrency(string key);
        public int GetCurrencyAmount(string key);
    }
}