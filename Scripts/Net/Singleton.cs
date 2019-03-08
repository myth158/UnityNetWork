namespace Myth.Net.Utils
{
    public class Singleton<T> where T: Singleton<T>, new()
    {
        private static T _Instance;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new T();
                }
                return _Instance;
            }
        }
        
    }
}
