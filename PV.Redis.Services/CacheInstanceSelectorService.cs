using System;
using PV.Redis.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace PV.Redis.Services
{
    public class CacheInstanceSelectorService : ICacheInstanceSelectorService
    {
        private bool _wasWarned;
        private readonly IDummyCacheInstance _dummyCacheInstance;
        private readonly IDefaultCacheInstance _defaultCacheInstance;
        private readonly Action<Exception> _onFail;
        private readonly ILogger _logger;

        public CacheInstanceSelectorService(
             IDefaultCacheInstance defaultCacheInstance
            , Action<Exception> onFail
            , ILogger log
        )
        {
            _dummyCacheInstance = new DummyCacheInstance();
            _defaultCacheInstance = defaultCacheInstance;
            _onFail = onFail ?? throw new ArgumentNullException(nameof(onFail));
            _logger = log;
        }

        public IRedisCacheService GetCacheInstance()
        {
            try
            {
                if (CacheStatus.Information.CacheServerIsUp)
                    return _defaultCacheInstance;

                if (CacheStatus.Information.LastReconnectionAttempt.AddSeconds(30) < DateTime.Now)
                {
                    CacheStatus.Information.LastReconnectionAttempt = DateTime.Now;

                    _defaultCacheInstance.Connect();

                    CacheStatus.Information.CacheServerIsUp = true;
                    _wasWarned = false;
                }
                else
                {
                    return _dummyCacheInstance;
                }

                return _defaultCacheInstance;
            }
            catch (Exception ex)
            {
                if (!_wasWarned)
                {
                    try
                    {
                        _wasWarned = true;
                        _onFail(ex);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                _logger.LogError("Servidor REDIS está fora do ar.{0}Exception: {1}", Environment.NewLine, ex);

                return _dummyCacheInstance;
            }
        }
    }
}