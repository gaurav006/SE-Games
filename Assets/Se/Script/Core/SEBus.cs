using System;
using System.Collections.Generic;
using UnityEngine;

namespace SEGames
{
    public static class SEBus
    {
       
        private static readonly Dictionary<Type, List<Delegate>> _listeners
            = new Dictionary<Type, List<Delegate>>();

        public static void Listen<T>(Action<T> callback)
        {
            if (callback == null) return;
            var key = typeof(T);
            if (!_listeners.TryGetValue(key, out var list))
            {
                list = new List<Delegate>();
                _listeners[key] = list;
            }
            if (!list.Contains(callback))
                list.Add(callback);
        }

        public static void Unlisten<T>(Action<T> callback)
        {
            if (callback == null) return;
            if (_listeners.TryGetValue(typeof(T), out var list))
                list.Remove(callback);
        }

        public static void Emit<T>(T eventData)
        {
            if (!_listeners.TryGetValue(typeof(T), out var list)) return;
            var snapshot = new List<Delegate>(list);
            foreach (var d in snapshot)
            {
                try
                {
                    ((Action<T>)d)?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SEBus] Exception in listener for {typeof(T).Name}: {e}");
                }
            }
        }

      
        public static void ListenOnce<T>(Action<T> callback)
        {
            Action<T> wrapper = null;
            wrapper = (data) =>
            {
                Unlisten<T>(wrapper);
                callback(data);
            };
            Listen<T>(wrapper);
        }
    }
}