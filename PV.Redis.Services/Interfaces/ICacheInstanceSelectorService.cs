namespace PV.Redis.Services.Interfaces
{
    public interface ICacheInstanceSelectorService
    {
        IRedisCacheService GetCacheInstance();
    }
}