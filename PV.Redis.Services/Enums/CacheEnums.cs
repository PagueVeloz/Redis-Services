using System;

namespace PV.Redis.Services.Enums
{
    public static class CacheEnums
    {
        //Cópia dos Enums do StackExchange.Redis para evitar ter que adicionar a dependência do pacote em vários projetos
        //Esses enums são convertidos internamente nos enums originais do CacheService

        public enum When
        {
            // Summary:
            //     The operation should occur whether or not there is an existing value
            Always = 0,

            // Summary:
            //     The operation should only occur when there is an existing value
            Exists = 1,

            // Summary:
            //     The operation should only occur when there is not an existing value
            NotExists = 2
        }

        [Flags]
        public enum CommandFlags
        {
            // Summary:
            //     Default behaviour.
            None = 0,

            // Summary:
            //     This operation should be performed on the master if it is available, but read
            //     operations may be performed on a slave if no master is available. This is the
            //     default option.
            PreferMaster = 0,

            // Summary:
            //     This command may jump regular-priority commands that have not yet been written
            //     to the redis stream.
            HighPriority = 1,

            // Summary:
            //     The caller is not interested in the result; the caller will immediately receive
            //     a default-value of the expected return type (this value is not indicative of
            //     anything at the server).
            FireAndForget = 2,

            // Summary:
            //     This operation should only be performed on the master.
            DemandMaster = 4,

            // Summary:
            //     This operation should be performed on the slave if it is available, but will
            //     be performed on a master if no slaves are available. Suitable for read operations
            //     only.
            PreferSlave = 8,

            // Summary:
            //     This operation should only be performed on a slave. Suitable for read operations
            //     only.
            DemandSlave = 12,

            // Summary:
            //     Indicates that this operation should not be forwarded to other servers as a result
            //     of an ASK or MOVED response
            NoRedirect = 64
        }
    }
}