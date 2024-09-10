namespace LocalizationSystem
{
    public interface ILocalizationManager
    {
        public void Init(string language);
        public string Get(string key);
        public string Get(string key, params object[] args);
    }
}