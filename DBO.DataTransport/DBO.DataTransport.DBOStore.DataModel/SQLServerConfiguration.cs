using System;
using System.Collections.Generic;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class SQLServerConfiguration
    {
        public long ConnectId { get; set; }
        public long ConfigId { get; set; }
        public string DataSource { get; set; }
        public string ProtectedPassword { get; set; }
        public string UserId { get; set; }
        public string Application { get; set; }
        public bool IsSecurityIntegrated { get; set; }
        public string Provider { get; set; }
        public bool AsynchronousProcessing { get; set; }

        public virtual RDBMSConfiguration Config { get; set; }
    }
}
