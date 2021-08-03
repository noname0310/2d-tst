using System;
using System.Collections.Generic;

namespace Assets.Scripts.Event
{
    [Serializable]
    public class IntegrityDictionary<TK, TV> where TV : new()
    {
        public TV this[TK key]
        {
            get
            {
                if (_dictionary.TryGetValue(key, out var value))
                    return value;
                value = new TV();
                _dictionary.Add(key, value);
                return value;
            }
            set
            {
                if (_dictionary.ContainsKey(key))
                    _dictionary[key] = value;
                else
                    _dictionary.Add(key, value);
            }
        }

        public void Clear() => _dictionary.Clear();

        private readonly Dictionary<TK, TV> _dictionary;

        public IntegrityDictionary() => _dictionary = new Dictionary<TK, TV>();
    }
}
