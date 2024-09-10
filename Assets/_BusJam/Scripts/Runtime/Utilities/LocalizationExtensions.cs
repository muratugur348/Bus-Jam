using LocalizationSystem;

namespace BusJam.Scripts.Runtime.Utilities
{
    public static class LocalizationExtensions
    {
        public static string ToLocalize(this string key)
        {
            var localizationManager = Locator.Instance.Resolve<ILocalizationManager>();
            return localizationManager.Get(key);
        }
        
        public static string ToLocalize(this string key, params object[] args)
        {
            var localizationManager = Locator.Instance.Resolve<ILocalizationManager>();
            return localizationManager.Get(key, args);
        }
    }
}