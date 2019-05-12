using System;
using System.Collections.Generic;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class ActionType
    {
        public ActionType()
        {
            DbotransportHistory = new HashSet<DBOTransportHistory>();
        }

        public byte TypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DBOTransportHistory> DbotransportHistory { get; set; }
    }
}
