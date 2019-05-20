using System;
using System.Collections.Generic;
using System.Text;

namespace DBO.DataTransport.Common.MappingModels
{
    public class ProjectMappingModel
    {
        public long ProjectId { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Description { get; set; }
    }
}
