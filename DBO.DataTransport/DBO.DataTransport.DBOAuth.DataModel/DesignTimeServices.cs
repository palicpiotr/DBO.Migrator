using DBO.DataTransport.DBOAuth.DataModel.Pluralazing;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace DBO.DataTransport.DBOAuth.DataModel
{
    public class DesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPluralizer, Pluralizer>();
        }
    }
}
