using System;
using System.Collections.Generic;
using System.Text;

namespace DBO.DataTransport.Common.CreateModels
{
    public class CreateSQLServerConfigModel
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
    }
}
