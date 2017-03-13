using System;
using System.Linq;
using PV.Redis.Services.Enums;
using PV.Redis.Services.Interfaces;
using PV.Redis.Services.Models;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace PV.Redis.Services
{
    internal class RedisCacheProvider : ICacheProvider
    {
        private readonly IPVRedisSettings _settings;
        private readonly ILogger _logger;
        private static ConnectionMultiplexer _redis;

        public RedisCacheProvider(
            ILogger logger
            , IPVRedisSettings settings
        )
        {
            _settings = settings;
            _logger = logger;
        }

        public void Connect()
        {
            try
            {
                var endpoint = GetSlaveServerPort();
                if (_redis == null)
                    _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
                    {
                        EndPoints =
                        {
                            {
                                _settings.RedisMasterEndPointHost,
                                _settings.RedisMasterEndPointPort
                            },
                            {
                                endpoint.Host,
                                endpoint.Port
                            }
                        },
                        Password = _settings.RedisEndPointPassword,
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DefaultCacheInstance)}: Erro ao conectar a instância do Redis.{Environment.NewLine}{ex}");
                CacheStatus.Information.CacheServerIsUp = false;
                throw;
            }
        }

        public virtual bool Set(string key, string value, TimeSpan? expires, CacheEnums.When when,
            CacheEnums.CommandFlags command)
        {
            return _redis.GetDatabase().StringSet(key, value, expires, ToWhen(when), ToCommandFlags(command));
        }

        public virtual string Get(string key, CacheEnums.CommandFlags command)
        {
            return _redis.GetDatabase().StringGet(key, ToCommandFlags(command));
        }

        public virtual string GetSet(string key, string value, CacheEnums.CommandFlags command)
        {
            return _redis.GetDatabase().StringGetSet(key, value, ToCommandFlags(command));
        }

        public virtual bool KeyDelete(string key, CacheEnums.CommandFlags command)
        {
            return _redis.GetDatabase().KeyDelete(key, ToCommandFlags(command));
        }

        private When ToWhen(CacheEnums.When when)
        {
            return (When)when;
        }

        private CommandFlags ToCommandFlags(CacheEnums.CommandFlags flags)
        {
            return (CommandFlags)flags;
        }

        private RedisEndPoint GetSlaveServerPort()
        {
            var key =
                _settings.RedisSlaveAddress.Keys.SingleOrDefault(k => k.ToUpper()
                    .Contains(Environment.MachineName.ToUpper())) ?? _settings.RedisSlaveAddress.Keys.First();

            return _settings.RedisSlaveAddress[key];
        }
    }
}