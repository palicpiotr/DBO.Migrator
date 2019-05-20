using DBO.DataTransport.Common.MappingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBO.DataTransport.DBOStore.DataAccess.Providers
{
    public interface ISupportedProvider
    {
        IQueryable<SupportedRDBMSMappingModel> GetSupportedRDBMS();
    }
}
