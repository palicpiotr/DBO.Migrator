using DBO.DataTransport.Common.CreateModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DBO.DataTransport.DBOStore.DataAccess.Providers
{
    public interface ISQLServerConfigProvider
    {
        Task CreateSqlServerConfiguration(CreateSQLServerConfigModel configModel);
    }
}
