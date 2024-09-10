using System;
using System.Collections.Generic;
using UnityEngine;

namespace BusJam.Scripts.Runtime
{
    public class Locator
    {
        static          Locator                  _instance;
        static          object                   _syncRoot  = new();
        static readonly Dictionary<Type, object> _managers = new();

        public static Locator Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (_syncRoot)
                {
                    _instance ??= new Locator();
                }

                return _instance;
            }
        }

        public void Register<T>(object manager)
        {
            lock (_syncRoot)
            {
                if (_managers.ContainsKey(typeof(T)))
                {
                    _managers[typeof(T)] = manager;
                }
                else
                {
                    _managers.Add(typeof(T), manager);
                }
            }
        }

        public T Resolve<T>()
        {
            lock (_syncRoot)
            {
                if (!_managers.TryGetValue(typeof(T), out object manager))
                {
                    Debug.LogError($"Manager not found {(typeof(T).Name)}");
                }

                return (T)manager;
            }
        }

        public void Reset()
        {
            lock (_syncRoot)
            {
                _managers.Clear();
            }
        }
    }
}