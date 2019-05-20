using DBO.DataTransport.Common.CreateModels;
using DBO.DataTransport.DBOStore.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DBO.DataTransport.DBOStore.DataAccess.Providers
{
    public class SQLServerConfigProvider : ISQLServerConfigProvider
    {
        private readonly DBOStoreContext _ctx;

        public SQLServerConfigProvider(DBOStoreContext ctx)
        {
            _ctx = ctx;
        }

        public async Task CreateSqlServerConfiguration(CreateSQLServerConfigModel configModel)
        {
            var config = new SQLServerConfiguration
            {
                Application = configModel.Application,
                AsynchronousProcessing = configModel.AsynchronousProcessing,
                ConfigId = configModel.ConfigId,
                DataSource = configModel.DataSource,
                IsSecurityIntegrated = configModel.IsSecurityIntegrated,
                ProtectedPassword = configModel.ProtectedPassword,
                Provider = configModel.Provider,
                UserId = configModel.UserId
            };
            await _ctx.SqlserverConfigurations.AddAsync(config);
            await _ctx.SaveChangesAsync(CancellationToken.None);
        }
    }
}
