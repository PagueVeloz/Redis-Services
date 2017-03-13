using System;
using PV.Redis.Services.Enums;

namespace PV.Redis.Services.Interfaces
{
    public interface ICacheProvider
    {
        void Connect();

        string Get(string key, CacheEnums.CommandFlags command);

        string GetSet(string key, string value, CacheEnums.CommandFlags command);

        bool Set(string key, string value, TimeSpan? expires, CacheEnums.When when, CacheEnums.CommandFlags command);

        bool KeyDelete(string key, CacheEnums.CommandFlags command);
    }
}