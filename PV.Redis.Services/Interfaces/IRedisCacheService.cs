using System;
using PV.Redis.Services.Enums;

namespace PV.Redis.Services.Interfaces
{
    public interface IRedisCacheService
    {
        void Connect();

        T GetAndSetIfNull<T>(string key, Func<T> fnGetDbValue, TimeSpan? expires = null, CacheEnums.When when = CacheEnums.When.Always, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None);

        T GetAndSetIfNull<T>(string key, T valueToAdd, TimeSpan? expires = null, CacheEnums.When when = CacheEnums.When.Always, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None);

        bool Set<T>(string key, T value, TimeSpan? expires = null, CacheEnums.When when = CacheEnums.When.Always, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None);

        T Get<T>(string key, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None);

        T GetAndSet<T>(string key, T value, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None);

        bool RemoveByKey<T>(string key, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None);

        string GetQualifiedKey<T>(string key);
    }
}