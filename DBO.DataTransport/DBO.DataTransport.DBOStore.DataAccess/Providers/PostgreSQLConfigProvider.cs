using DBO.DataTransport.Common.CreateModels;
using DBO.DataTransport.DBOStore.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DBO.DataTransport.DBOStore.DataAccess.Providers
{
    public class PostgreSQLConfigProvider : IPostgreSQLConfigProvider
    {

        private readonly DBOStoreContext _ctx;

        public PostgreSQLConfigProvider(DBOStoreContext ctx)
        {
            _ctx = ctx;
        }

        public async Task CreatePostgreSqlConfiguration(CreatePostgreSQLConfigModel configModel)
        {
            var config = new PostgreSQLConfiguration
            {
                ConfigId = configModel.ConfigId,
                ConnectionLifetime = configModel.ConnectionLifetime,
                Database = configModel.Database,
                Host = configModel.Host,
                MaxPoolSize = configModel.MaxPoolSize,
                MinPoolSize = configModel.MinPoolSize,
                Pooling = configModel.Pooling,
                Port = configModel.Port,
                ProtectedPassword = configModel.ProtectedPassword,
                UserId = configModel.UserId
            };
            await _ctx.PostgreSqlconfigurations.AddRangeAsync(config);
            await _ctx.SaveChangesAsync(CancellationToken.None);
        }
    }
}
