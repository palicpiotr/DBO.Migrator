using System;
using System.Collections.Generic;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class SupportedRDBMS
    {
        public SupportedRDBMS()
        {
            ProjectRDBMSRelationshipRdbmsconsumer = new HashSet<ProjectRDBMSRelationship>();
            ProjectRDBMSRelationshipRdbmsowner = new HashSet<ProjectRDBMSRelationship>();
        }

        public byte Rdbmsid { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProjectRDBMSRelationship> ProjectRDBMSRelationshipRdbmsconsumer { get; set; }
        public virtual ICollection<ProjectRDBMSRelationship> ProjectRDBMSRelationshipRdbmsowner { get; set; }
    }
}
