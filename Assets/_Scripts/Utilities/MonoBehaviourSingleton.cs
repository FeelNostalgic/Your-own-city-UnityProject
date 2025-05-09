using UnityEngine;

namespace Utilities
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance == null) _instance = (T) FindObjectOfType(typeof(T));
                return _instance;
            }
        }
    }
}