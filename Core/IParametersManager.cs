using System;

namespace Paragon.Container.Core
{
    public interface IParametersManager
    {
        void Set<T>(string key, T value);
        T Get<T>(string key);
        bool ContainsKey(string key);
    }
}
