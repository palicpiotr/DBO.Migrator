using System;
using System.Collections.Generic;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class DBOTransportHistory
    {
        public long Id { get; set; }
        public byte ActionTypeId { get; set; }
        public long ProjectId { get; set; }
        public string Notes { get; set; }

        public virtual ActionType ActionType { get; set; }
        public virtual Project Project { get; set; }
    }
}
