using System;
using System.Collections.Generic;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class ProjectRDBMSRelationship
    {
        public long RelationId { get; set; }
        public long ProjectId { get; set; }
        public byte RdbmsownerId { get; set; }
        public byte RdbmsconsumerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual Project Project { get; set; }
        public virtual SupportedRDBMS Rdbmsconsumer { get; set; }
        public virtual SupportedRDBMS Rdbmsowner { get; set; }
    }
}
