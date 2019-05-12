using System;
using System.Collections.Generic;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class PostgreSQLConfiguration
    {
        public long ConnectId { get; set; }
        public long ConfigId { get; set; }
        public string UserId { get; set; }
        public string ProtectedPassword { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public bool? Pooling { get; set; }
        public byte? MinPoolSize { get; set; }
        public byte? MaxPoolSize { get; set; }
        public byte? ConnectionLifetime { get; set; }

        public virtual RDBMSConfiguration Config { get; set; }
    }
}
