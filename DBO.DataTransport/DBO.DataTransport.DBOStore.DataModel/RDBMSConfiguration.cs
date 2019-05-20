using System;
using System.Collections.Generic;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class RDBMSConfiguration
    {
        public RDBMSConfiguration()
        {
            PostgreSqlconfigurations = new HashSet<PostgreSQLConfiguration>();
            SqlserverConfigurations = new HashSet<SQLServerConfiguration>();
        }

        public long ConfigId { get; set; }
        public long ProjectId { get; set; }
        public byte Rdmbsid { get; set; }
        public string Name { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<PostgreSQLConfiguration> PostgreSqlconfigurations { get; set; }
        public virtual ICollection<SQLServerConfiguration> SqlserverConfigurations { get; set; }
    }
}
