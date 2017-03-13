using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PV.Redis.Services.Enums;
using PV.Redis.Services.Interfaces;
using StackExchange.Redis;

namespace PV.Redis.Services
{
    public sealed class DefaultCacheInstance : IDefaultCacheInstance
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ILogger _logger;

        internal DefaultCacheInstance(
            ILogger logger
            , ICacheProvider cacheProvider
        )
        {
            _logger = logger;
            _cacheProvider = cacheProvider;
        }

        public void Connect()
        {
            _cacheProvider.Connect();
        }

        public T Get<T>(string key, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            try
            {
                key = GetQualifiedKey<T>(key);

                var value = _cacheProvider.Get(key, flags | CacheEnums.CommandFlags.PreferSlave);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                HandleException(nameof(Get), ex);
                throw;
            }
        }

        public bool Set<T>(string key, T value, TimeSpan? expires = null, CacheEnums.When when = CacheEnums.When.Always,
            CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            try
            {
                key = GetQualifiedKey<T>(key);
                return _cacheProvider.Set(key, JsonConvert.SerializeObject(value), expires, when, flags);
            }
            catch (Exception ex)
            {
                HandleException(nameof(Set), ex);
                throw;
            }
        }

        public T GetAndSetIfNull<T>(string key, Func<T> fnGetDbValue, TimeSpan? expires = null,
            CacheEnums.When when = CacheEnums.When.Always, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            try
            {
                var value = Get<T>(key);

                if (value != null) return value;

                value = fnGetDbValue();

                Set(key, value, expires, when, flags);

                return value;
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAndSetIfNull), ex);
                throw;
            }
        }

        public T GetAndSetIfNull<T>(string key, T valueToAdd, TimeSpan? expires = null,
            CacheEnums.When when = CacheEnums.When.Always, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            return GetAndSetIfNull(key, () => valueToAdd, expires, when, flags);
        }

        public T GetAndSet<T>(string key, T value, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            try
            {
                var serializedValue = _cacheProvider.GetSet(key, JsonConvert.SerializeObject(value), flags);

                return JsonConvert.DeserializeObject<T>(serializedValue);
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAndSet), ex);
                throw;
            }
        }

        public bool RemoveByKey<T>(string key, CacheEnums.CommandFlags flags = CacheEnums.CommandFlags.None)
        {
            try
            {
                key = GetQualifiedKey<T>(key);

                return _cacheProvider.KeyDelete(key, flags);
            }
            catch (Exception ex)
            {
                HandleException(nameof(RemoveByKey), ex);
                throw;
            }
        }

        private When ToWhen(CacheEnums.When when)
        {
            return (When)when;
        }

        private CommandFlags ToCommandFlags(CacheEnums.CommandFlags flags)
        {
            return (CommandFlags)flags;
        }

        public string GetQualifiedKey<T>(string key)
        {
            var type = typeof(T);
            key = key.Replace(":", string.Empty);

            if (type.GetTypeInfo().IsPrimitive || type == typeof(string))
                return $"Default:{key}";
            else
                return $"{type.Name}:{key}";
        }

        private void HandleException(string errorNamespace, Exception ex)
        {
            _logger.LogError($"{nameof(DefaultCacheInstance)}.{errorNamespace}{Environment.NewLine}{ex}");
            CacheStatus.Information.CacheServerIsUp = false;
        }
    }
}