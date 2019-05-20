using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBO.DataTransport.DBOStore.DataAccess.Providers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBO.DataTransport.FE.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {

        private readonly IProjectProvider _projectProvider;

        public ProjectController(IProjectProvider projectProvider)
        {
            _projectProvider = projectProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProjects([DataSourceRequest]DataSourceRequest request)
        {
            var projects = _projectProvider.GetAvailableProjects(User.Identity.Name);
            return Json(await projects.ToDataSourceResultAsync(request));
        }



    }
}
