using System;
using System.Collections.Generic;

namespace Paragon.Container.Core
{
    internal sealed class ParametersManager : IParametersManager
    {
        public void Set<T>(string key, T value)
        {
            _data[key] = value;
        }
        public T Get<T>(string key)
        {
            return (T)_data[key];
        }
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        private Dictionary<string, object> _data = new Dictionary<string, object>();
    }
}
