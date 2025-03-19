namespace Utilities
{
    public class Singlenton<T> where T : new()
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance == null) _instance = (T) new T();
                return _instance;
            }
        }
    }
}