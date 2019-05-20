using DBO.DataTransport.Common.MappingModels;
using DBO.DataTransport.DBOStore.DataModel;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DBO.DataTransport.DBOStore.DataAccess.Providers
{
    public class ProjectProvider : IProjectProvider
    {

        private readonly DBOStoreContext _ctx;

        public ProjectProvider(DBOStoreContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<ProjectMappingModel> GetAvailableProjects(string token)
        {
            return _ctx.Projects.Where(x => x.OwnerId == token)
                .Select(x => new ProjectMappingModel
                {
                    ProjectId = x.ProjectId,
                    OwnerId = x.OwnerId,
                    CreationDate = x.CreationDate,
                    Description = x.Description,
                    Name = x.Name,
                    Type = x.Type,
                    UpdateDate = x.UpdateDate
                });
        }

        public async Task CreateNewProject(string name, byte type, string token, string description)
        {
            var project = new Project
            {
                Name = name,
                Type = type,
                CreationDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Description = description,
                OwnerId = token
            };
            await _ctx.Projects.AddAsync(project);
            await _ctx.SaveChangesAsync(CancellationToken.None);
        }

        public async Task CreateProjectConfiguration(long projectId, byte rdmsId, string name)
        {
            var config = new RDBMSConfiguration
            {
                ProjectId = projectId,
                Rdmbsid = rdmsId,
                Name = name
            };
            await _ctx.Rdbmsconfigurations.AddAsync(config);
            await _ctx.SaveChangesAsync(CancellationToken.None);
        }

    }
}
