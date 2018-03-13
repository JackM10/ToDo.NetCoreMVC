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
using ToDoNetCore.Controllers;

namespace ToDoNetCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddResponseCompression();

            var connection =
                @"Server=(localdb)\MSSQLLocalDB;Database=ToDoNetCore;Trusted_Connection=True;";
            services.AddDbContext<ToDoContext>(options => options.UseSqlServer(connection));
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
