using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class MonoBehaviourSingletonTemplate<T> : MonoBehaviour where T : MonoBehaviourSingletonTemplate<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        protected virtual void Awake() {
            Debug.Assert(instance == null);
            instance = (T) this;
            GameObject.DontDestroyOnLoad(this);
        }

        protected virtual void OnDestroy() {
            instance = null;
        }
    }
}
