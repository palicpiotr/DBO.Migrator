using DBO.DataTransport.Common.MappingModels;
using System.Linq;
using System.Threading.Tasks;

namespace DBO.DataTransport.DBOStore.DataAccess.Providers
{
    public interface IProjectProvider
    {
        IQueryable<ProjectMappingModel> GetAvailableProjects(string token);
        Task CreateNewProject(string name, byte type, string token, string description);
        Task CreateProjectConfiguration(long projectId, byte rdmsId, string name);
    }
}
