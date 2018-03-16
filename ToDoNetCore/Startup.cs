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

namespace ToDoNetCore
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env) => Configuration = (new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables()).Build();
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddResponseCompression();
            services.AddTransient<IToDoRepository, EFToDoRepository>();
            services.AddDbContext<ToDoContext>(options => 
                options.UseSqlServer(Configuration["Data:ToDoNetCore:ConnectionString"]));
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

                //controller.MapRoute(
                //    "CatchError",
                //    "Errors{errorCode}",
                //    new { controller = "ToDo", action = "Errors" });
            });

            app.UseStaticFiles();

            app.UseStatusCodePagesWithReExecute("/Errors/{0}");

            app.UseResponseCompression();

            //loggerFactory.AddFile("ToDo_{Date}.txt");
        }
    }
}
