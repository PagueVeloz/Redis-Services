using System;
using PV.Redis.Services.Enums;
using PV.Redis.Services.Interfaces;

namespace PV.Redis.Services
{
    internal class DummyCacheInstance : IDummyCacheInstance
    {
        public void Connect()
        {
        }

        public T Get<T>(string key, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            return default(T);
        }

        public T GetAndSet<T>(string key, T value, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            return value;
        }

        public T GetAndSetIfNull<T>(string key, T valueToAdd, TimeSpan? expires = default(TimeSpan?), CacheEnums.When when = CacheEnums.When.Always, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            return valueToAdd;
        }

        public T GetAndSetIfNull<T>(string key, Func<T> fnGetDbValue, TimeSpan? expires = default(TimeSpan?), CacheEnums.When when = CacheEnums.When.Always, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            return fnGetDbValue();
        }

        public string GetQualifiedKey<T>(string key)
        {
            return null;
        }

        public bool RemoveByKey<T>(string key, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            return true;
        }

        public bool Set<T>(string key, T value, TimeSpan? expires = default(TimeSpan?), CacheEnums.When when = CacheEnums.When.Always, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            return true;
        }
    }
}