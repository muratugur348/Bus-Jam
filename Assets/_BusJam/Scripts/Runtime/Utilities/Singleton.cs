namespace BusJam.Scripts.Utilities
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object _lockObject = new object();

        public static T Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }

                    return _instance;
                }
            }
        }
    }

}