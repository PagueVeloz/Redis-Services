using System.Collections.Generic;
using PV.Redis.Services.Models;

namespace PV.Redis.Services.Interfaces
{
    // ReSharper disable once InconsistentNaming
    public interface IPVRedisSettings
    {
        string RedisMasterEndPointHost { get; }

        int RedisMasterEndPointPort { get; }

        string RedisEndPointPassword { get; }

        IDictionary<string, RedisEndPoint> RedisSlaveAddress { get; }
    }
}