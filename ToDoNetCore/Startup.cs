using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDoNetCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ToDoNetCore.Controllers;
using ToDoNetCore.Infrastructure;

namespace ToDoNetCore
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env) => Configuration = (new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables()).Build();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddResponseCompression();
            services.AddTransient<IToDoRepository, EFToDoRepository>();
            services.AddDbContext<ToDoContext>(options => 
                options.UseSqlServer(Configuration["Data:ToDoNetCore:ConnectionString"]));
            services.AddSingleton<ApplicationUptime>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsProduction())
            {
                loggerFactory.AddFile("ToDo_{Date}.txt");
            }

            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseMiddleware<StatusCodeHandlingMiddleware>();
            app.UseResponseCompression();

            app.UseMvc(controller =>
            {
                controller.MapRoute(
                    "ViewOneToDo",
                    "ToDos/ID-{id:int}",
                    new {controller = "ToDo", action = "ViewOneItem"});

                controller.MapRoute(
                    "default", 
                    "{controller}/{action}/{id?}",
                    new {controller = "ToDo", action = "List"});
            });
        }
    }
}
