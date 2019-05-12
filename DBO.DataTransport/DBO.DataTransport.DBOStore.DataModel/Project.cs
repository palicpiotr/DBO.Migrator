using System;
using System.Collections.Generic;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class Project
    {
        public Project()
        {
            DbotransportHistory = new HashSet<DBOTransportHistory>();
            ProjectRdbmsrelationships = new HashSet<ProjectRDBMSRelationship>();
            Rdbmsconfigurations = new HashSet<RDBMSConfiguration>();
        }

        public long ProjectId { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DBOTransportHistory> DbotransportHistory { get; set; }
        public virtual ICollection<ProjectRDBMSRelationship> ProjectRdbmsrelationships { get; set; }
        public virtual ICollection<RDBMSConfiguration> Rdbmsconfigurations { get; set; }
    }
}
