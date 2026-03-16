using System;
using System.Collections.Generic;
using UnityEngine;

namespace SEGames
{
    public static class SEState
    {
        public static bool AutoFlush { get; set; } = false;

        private static readonly Dictionary<string, string> _cache
            = new Dictionary<string, string>();

        private static readonly HashSet<string> _dirty
            = new HashSet<string>();

        public static void Set<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("[SEState] Set() called with a null or empty key. Ignored.");
                return;
            }
            string json;
            try
            {
                json = JsonUtility.ToJson(new Wrapper<T>(value));
            }
            catch (Exception e)
            {
                Debug.LogError($"[SEState] Failed to serialise value for key '{key}': {e.Message}");
                return;
            }
            _cache[key] = json;
            _dirty.Add(key);
            if (AutoFlush)
                FlushKey(key, json);
        }

        public static T Get<T>(string key, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("[SEState] Get() called with a null or empty key. Returning default.");
                return defaultValue;
            }
            if (_cache.TryGetValue(key, out string cached))
                return Deserialise<T>(key, cached, defaultValue);

            if (PlayerPrefs.HasKey(key))
            {
                string stored = PlayerPrefs.GetString(key);
                _cache[key] = stored;
                return Deserialise<T>(key, stored, defaultValue);
            }
            return defaultValue;
        }

        public static bool Has(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            return _cache.ContainsKey(key) || PlayerPrefs.HasKey(key);
        }

        public static void Flush()
        {
            if (_dirty.Count == 0) return;
            foreach (string key in _dirty)
            {
                if (_cache.TryGetValue(key, out string json))
                    PlayerPrefs.SetString(key, json);
            }
            PlayerPrefs.Save();
            _dirty.Clear();
        }

        public static void Delete(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            _cache.Remove(key);
            _dirty.Remove(key);
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }

        public static void Clear()
        {
            _cache.Clear();
            _dirty.Clear();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        private static void FlushKey(string key, string json)
        {
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
            _dirty.Remove(key);
        }

        private static T Deserialise<T>(string key, string json, T defaultValue)
        {
            try
            {
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
                return wrapper != null ? wrapper.value : defaultValue;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SEState] Failed to deserialise value for key '{key}': {e.Message}. Returning default.");
                return defaultValue;
            }
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T value;
            public Wrapper(T v) { value = v; }
        }
    }
}