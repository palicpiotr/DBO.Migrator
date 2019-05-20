using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBO.DataTransport.FE.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DBO.DataTransport.DBOStore.DataModel;
using DBO.DataTransport.DBOAuth.DataModel;
using DBO.DataTransport.DBOStore.DataAccess.Providers;
using DBO.DataTransport.FE.Controllers;

namespace DBO.DataTransport.FE
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            Configuration = config;
            Environment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("AuthConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddKendo();

            return BuildAutofacContainer(services);
        }

        private IServiceProvider BuildAutofacContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.Register(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<DBOStoreContext>()
                           .UseSqlServer(Configuration.GetConnectionString("StoreConnection"));
                return new DBOStoreContext(optionsBuilder.Options);
            }).As<DBOStoreContext>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<DBOAuthContext>()
                            .UseSqlServer(Configuration.GetConnectionString("AuthConnection"));
                return new DBOAuthContext(optionsBuilder.Options);
            }).As<DBOAuthContext>().InstancePerLifetimeScope();

            builder.RegisterType<ProjectProvider>().As<IProjectProvider>().InstancePerDependency();
            builder.RegisterType<PostgreSQLConfigProvider>().As<IPostgreSQLConfigProvider>().InstancePerDependency();
            builder.RegisterType<SQLServerConfigProvider>().As<ISQLServerConfigProvider>().InstancePerDependency();
            builder.RegisterType<SupportedProvider>().As<ISupportedProvider>().InstancePerDependency();

            builder.RegisterType<HomeController>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectController>().InstancePerLifetimeScope();

            builder.Populate(services);
            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
