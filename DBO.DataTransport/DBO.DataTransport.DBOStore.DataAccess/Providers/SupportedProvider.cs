using DBO.DataTransport.Common.MappingModels;
using DBO.DataTransport.DBOStore.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBO.DataTransport.DBOStore.DataAccess.Providers
{
    public class SupportedProvider : ISupportedProvider
    {
        private readonly DBOStoreContext _ctx;

        public SupportedProvider(DBOStoreContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<SupportedRDBMSMappingModel> GetSupportedRDBMS()
        {
            return _ctx.SupportedRdbms.Select(x => new SupportedRDBMSMappingModel
            {
                Rdbmsid = x.Rdbmsid,
                Name = x.Name
            });
        }

    }
}
