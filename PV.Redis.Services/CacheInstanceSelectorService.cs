using System;
using PV.Redis.Services.Interfaces;

namespace PV.Redis.Services
{
    public class CacheInstanceSelectorService : ICacheInstanceSelectorService
    {
        private bool _wasWarned;
        private readonly IDummyCacheInstance _dummyCacheInstance;
        private readonly IDefaultCacheInstance _defaultCacheInstance;
        private readonly Action<Exception> _onFail;
        private readonly ILog _logger;

        public CacheInstanceSelectorService(
            IDummyCacheInstance dummyCacheInstance
            , IDefaultCacheInstance defaultCacheInstance
            , Action<Exception> onFail
            , ILog log
        )
        {
            if (onFail == null)
                throw new ArgumentNullException(nameof(onFail));

            _dummyCacheInstance = dummyCacheInstance;
            _defaultCacheInstance = defaultCacheInstance;
            _onFail = onFail;
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

                _logger.Error("Servidor REDIS está fora do ar", ex);

                return _dummyCacheInstance;
            }
        }
    }
}